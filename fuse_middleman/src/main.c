#include "common.h"
#include "fuse_ops.h"

static struct fuse_operations ops = {
    .getattr = do_getattr,
    .readdir = do_readdir,
    .mkdir   = do_mkdir,
    .rmdir   = do_rmdir,
    .create  = do_create,
    .unlink  = do_unlink,
    .open    = do_open,
    .read    = do_read,
    .write   = do_write,
};

int main(int argc, char *argv[])
{
    if (argc < 2) {
        fprintf(stderr, "Usage: %s <mountpoint>\n", argv[0]);
        return 1;
    }
    struct fuse_args args = FUSE_ARGS_INIT(argc, argv);
    return fuse_main(args.argc, args.argv, &ops, NULL);
}
