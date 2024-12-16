#ifndef FUSE_OPS_H
#define FUSE_OPS_H

#include "common.h"

int do_getattr(const char *path, struct stat *stbuf, struct fuse_file_info *fi);
int do_readdir(const char *path, void *buf, fuse_fill_dir_t filler,
               off_t offset, struct fuse_file_info *fi, enum fuse_readdir_flags flags);
int do_mkdir(const char *path, mode_t mode);
int do_rmdir(const char *path);
int do_create(const char *path, mode_t mode, struct fuse_file_info *fi);
int do_unlink(const char *path);
int do_open(const char *path, struct fuse_file_info *fi);
int do_read(const char *path, char *buf, size_t size, off_t offset, struct fuse_file_info *fi);
int do_write(const char *path, const char *buf, size_t size, off_t offset, struct fuse_file_info *fi);

#endif
