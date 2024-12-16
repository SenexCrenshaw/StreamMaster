#ifndef IPC_H
#define IPC_H

#include "common.h"

int ipc_getattr(const char *path, int *is_dir, long *size);
int ipc_readdir(const char *path, char ***names, int **is_dirs, int *count);
int ipc_mkdir(const char *path);
int ipc_rmdir(const char *path);
int ipc_create(const char *path);
int ipc_unlink(const char *path);
int ipc_read(const char *path, off_t offset, size_t size, char *buf);
int ipc_write(const char *path, off_t offset, const char *buf, size_t size);

#endif
