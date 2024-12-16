#include "ipc.h"
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <errno.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

#define SERVER_IP "127.0.0.1"
#define SERVER_PORT 9999

static int connect_socket(void) {
    int sock = socket(AF_INET, SOCK_STREAM, 0);
    if (sock < 0) return -1;

    struct sockaddr_in addr;
    memset(&addr,0,sizeof(addr));
    addr.sin_family = AF_INET;
    addr.sin_port = htons(SERVER_PORT);
    if(inet_pton(AF_INET, SERVER_IP, &addr.sin_addr)<=0){
        close(sock);
        return -1;
    }

    if (connect(sock,(struct sockaddr*)&addr,sizeof(addr))<0) {
        close(sock);
        return -1;
    }

    return sock;
}

static int read_line(int fd, char *buf, size_t size) {
    size_t pos=0;
    while(pos<size-1) {
        ssize_t r = read(fd,buf+pos,1);
        if(r<=0)return -1;
        if(buf[pos]=='\n'){buf[pos]='\0';return (int)pos;}
        pos++;
    }
    buf[size-1]='\0';
    return (int)pos;
}

int ipc_getattr(const char *path, int *is_dir, long *size) {
    int sock=connect_socket();
    if(sock<0)return -EIO;
    dprintf(sock,"GETATTR %s\n",path);
    char line[256];
    if(read_line(sock,line,sizeof(line))<0){close(sock);return -EIO;}
    close(sock);

    if(strncmp(line,"OK",2)==0){
        char type[16]; long sz;
        if(sscanf(line,"OK %15s %ld",type,&sz)==2){
            *is_dir=(strcmp(type,"dir")==0)?1:0;
            *size=sz;
            return 0;
        }
        return -EIO;
    }else{
        return -ENOENT;
    }
}

int ipc_readdir(const char *path, char ***names, int **is_dirs, int *count) {
    int sock=connect_socket();
    if(sock<0)return -EIO;
    dprintf(sock,"READDIR %s\n",path);

    char line[256];
    if(read_line(sock,line,sizeof(line))<0){close(sock);return -EIO;}
    if(strncmp(line,"OK",2)!=0){close(sock);return -ENOENT;}

    int n;
    if(sscanf(line,"OK %d",&n)!=1){close(sock);return -EIO;}
    *count=n;
    *names=(char**)malloc(n*sizeof(char*));
    *is_dirs=(int*)malloc(n*sizeof(int));
    for(int i=0;i<n;i++){
        if(read_line(sock,line,sizeof(line))<0){close(sock);return -EIO;}
        char name[256],type[16];
        if(sscanf(line,"%255s %15s",name,type)!=2){close(sock);return -EIO;}
        (*names)[i]=strdup(name);
        (*is_dirs)[i]=(strcmp(type,"dir")==0)?1:0;
    }
    close(sock);
    return 0;
}

int ipc_mkdir(const char *path) {
    int sock=connect_socket();
    if(sock<0)return -EIO;
    dprintf(sock,"MKDIR %s\n",path);
    char line[256];
    if(read_line(sock,line,sizeof(line))<0){close(sock);return -EIO;}
    close(sock);
    return (strncmp(line,"OK",2)==0)?0:-EIO;
}

int ipc_rmdir(const char *path) {
    int sock=connect_socket();
    if(sock<0)return -EIO;
    dprintf(sock,"RMDIR %s\n",path);
    char line[256];
    if(read_line(sock,line,sizeof(line))<0){close(sock);return -EIO;}
    close(sock);
    return (strncmp(line,"OK",2)==0)?0:-EIO;
}

int ipc_create(const char *path) {
    int sock=connect_socket();
    if(sock<0)return -EIO;
    dprintf(sock,"CREATE %s\n",path);
    char line[256];
    if(read_line(sock,line,sizeof(line))<0){close(sock);return -EIO;}
    close(sock);
    return (strncmp(line,"OK",2)==0)?0:-EIO;
}

int ipc_unlink(const char *path) {
    int sock=connect_socket();
    if(sock<0)return -EIO;
    dprintf(sock,"UNLINK %s\n",path);
    char line[256];
    if(read_line(sock,line,sizeof(line))<0){close(sock);return -EIO;}
    close(sock);
    return (strncmp(line,"OK",2)==0)?0:-EIO;
}

int ipc_read(const char *path, off_t offset, size_t size, char *buf) {
    int sock=connect_socket();
    if(sock<0)return -EIO;
    dprintf(sock,"READ %s %lld %zu\n",path,(long long)offset,size);

    char line[256];
    if(read_line(sock,line,sizeof(line))<0){close(sock);return -EIO;}
    if(strncmp(line,"OK",2)==0){
        int readBytes;
        if(sscanf(line,"OK %d",&readBytes)!=1){close(sock);return -EIO;}
        if(readBytes>0){
            int total=0;
            while(total<readBytes){
                int r=(int)read(sock,buf+total,readBytes-total);
                if(r<=0)break;
                total+=r;
            }
            close(sock);
            return total;
        } else {
            close(sock);
            return 0;
        }
    } else {
        close(sock);
        return 0;
    }
}

int ipc_write(const char *path, off_t offset, const char *wbuf, size_t size) {
    int sock=connect_socket();
    if(sock<0)return -EIO;
    dprintf(sock,"WRITE %s %lld %zu\n",path,(long long)offset,size);
    if(write(sock,wbuf,size)!=(ssize_t)size){close(sock);return -EIO;}

    char line[256];
    if(read_line(sock,line,sizeof(line))<0){close(sock);return -EIO;}
    close(sock);

    return (strncmp(line,"OK",2)==0)?(int)size:-EIO;
}
