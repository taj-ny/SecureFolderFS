# Testing
The FUSE implementation can be tested **only on Linux** with the xfstools test suite.

> **Note**
> The following instructions have been tested on NixOS only and may not work on other distributions without minor changes.

### Prerequisites
- fuse3
- xfstools (v2023.10.29 or later)
- meson
- ninja
- at least 10GiB of free space

### FUSE configuration
The ``user_allow_other`` option must be present and not commented in ``/etc/fuse.conf``, otherwise the suite will not be able to access the filesystem.

### Compiling libfuse examples
The ``passthrough_ll`` example is required for running the test suite. Clone the [libfuse](https://github.com/libfuse/libfuse) repository and run the following commands in the repository directory to compile the examples.
```bash
mkdir build
cd build
meson setup
ninja
```

The ``passthrough_ll`` binary will be located at ``build/example/passthrough_ll``.

### Running the test suite
Launch the SecureFolderFS.Core.FUSE.Tests project and run the following commands in a shell.

```bash
export PASSTHROUGH_LL=$HOME/libfuse/build/example/passthrough_ll # Change this if necessary

export TEST_VAULT_MOUNTPOINT=$HOME/.local/share/SecureFolderFS/mount/sffs_test_vault_test
export SCRATCH_VAULT_MOUNTPOINT=$HOME/.local/share/SecureFolderFS/mount/sffs_test_vault_scratch

export TEST_VAULT_PASSTHROUGH_MOUNTPOINT=$(mktemp -d)
export SCRATCH_VAULT_PASSTHROUGH_MOUNTPOINT=$(mktemp -d)

export MOUNT_SCRIPT=$PWD/sffs_mount

cat << EOF | tee $MOUNT_SCRIPT
#!/bin/sh
exec $PASSTHROUGH_LL -ofsname="\$@"
EOF
chmod +x $MOUNT_SCRIPT

export TEST_DEV=non1
export TEST_DIR=$TEST_VAULT_PASSTHROUGH_MOUNTPOINT
export SCRATCH_DEV=non2
export SCRATCH_MNT=$SCRATCH_VAULT_PASSTHROUGH_MOUNTPOINT
export FSTYP=fuse
export FUSE_SUBTYP=.$MOUNT_SCRIPT
export MOUNT_OPTIONS="-osource=$SCRATCH_VAULT_MOUNTPOINT,allow_other,default_permissions"
export TEST_FS_MOUNT_OPTS="-osource=$TEST_VAULT_MOUNTPOINT,allow_other,default_permissions"

sudo -E -- /bin/sh -c "prlimit --pid=\$\$ --nofile=1048576 && xfstests-check"

rm $MOUNT_SCRIPT
sudo umount -l $TEST_VAULT_PASSTHROUGH_MOUNTPOINT
sudo umount -l $SCRATCH_VAULT_PASSTHROUGH_MOUNTPOINT
```