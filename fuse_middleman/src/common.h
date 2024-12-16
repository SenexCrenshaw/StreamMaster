#ifndef COMMON_H
#define COMMON_H

#define _GNU_SOURCE
#define FUSE_USE_VERSION 31

#include <fuse3/fuse.h>
#include <fuse3/fuse_opt.h>
#include <fuse3/fuse_lowlevel.h>

#include <stdio.h>
#include <string.h>
#include <errno.h>
#include <fcntl.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/types.h>

#define SOCKET_PATH "/tmp/fuse_middleman.sock"

#endif
