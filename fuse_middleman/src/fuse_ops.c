#include "fuse_ops.h"
#include "ipc.h"
#include <sys/stat.h>

int do_getattr(const char *path, struct stat *stbuf, struct fuse_file_info *fi)
{
    (void)fi;
    memset(stbuf, 0, sizeof(struct stat));

    int is_dir; long size;
    int res = ipc_getattr(path, &is_dir, &size);
    if (res!=0) return res;

    if (strcmp(path,"/")==0) {
        // root directory
        stbuf->st_mode = S_IFDIR | 0555;
        stbuf->st_nlink = 2;
        return 0;
    }

    if (is_dir) {
        stbuf->st_mode = S_IFDIR | 0555;
        stbuf->st_nlink = 2;
    } else {
        stbuf->st_mode = S_IFREG | 0444;
        stbuf->st_size = size;
        stbuf->st_nlink = 1;
    }
    return 0;
}

int do_readdir(const char *path, void *buf, fuse_fill_dir_t filler,
               off_t offset, struct fuse_file_info *fi, enum fuse_readdir_flags flags)
{
    (void) offset;
    (void) fi;
    (void) flags;

    char **names;
    int *is_dirs;
    int count;

    // Add . and ..
    filler(buf, ".", NULL, 0, 0);
    filler(buf, "..", NULL, 0, 0);

    if (ipc_readdir(path, &names, &is_dirs, &count)==0) {
        for (int i=0; i<count; i++) {
            mode_t mode = is_dirs[i] ? (S_IFDIR|0555) : (S_IFREG|0444);
            struct stat st;
            memset(&st,0,sizeof(st));
            st.st_mode = mode;
            st.st_nlink = 1;
            filler(buf, names[i], &st, 0, 0);
            free(names[i]);
        }
        free(names);
        free(is_dirs);
        return 0;
    }
    return -ENOENT;
}

int do_mkdir(const char *path, mode_t mode)
{
    (void)mode;
    return ipc_mkdir(path);
}

int do_rmdir(const char *path)
{
    return ipc_rmdir(path);
}

int do_create(const char *path, mode_t mode, struct fuse_file_info *fi)
{
    (void)mode; (void)fi;
    return ipc_create(path);
}

int do_unlink(const char *path)
{
    return ipc_unlink(path);
}

int do_open(const char *path, struct fuse_file_info *fi)
{
    (void)fi;
    // Just check if file exists
    int is_dir; long size;
    if (ipc_getattr(path, &is_dir, &size)!=0 || is_dir)
        return -ENOENT;
    return 0;
}

int do_read(const char *path, char *buf, size_t size, off_t offset, struct fuse_file_info *fi)
{
    (void)fi;
    int res = ipc_read(path, offset, size, buf);
    return (res<0)?res:res;
}

int do_write(const char *path, const char *buf, size_t size, off_t offset, struct fuse_file_info *fi)
{
    (void)fi;
    int res = ipc_write(path, offset, buf, size);
    if (res<0) return res;
    return res;
}
