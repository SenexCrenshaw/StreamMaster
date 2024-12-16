CC = gcc
CFLAGS = -Wall -Wextra -O2 `pkg-config fuse3 --cflags`
LDFLAGS = `pkg-config fuse3 --libs`
SRC = src/fuse_ops.c src/ipc.c src/main.c
OBJ = $(SRC:.c=.o)
BIN = fuse_middleman

# You can override these when calling make, e.g.:
# make PREFIX=/usr install
PREFIX ?= /usr/local
BINDIR = $(PREFIX)/bin

all: $(BIN)

$(BIN): $(OBJ)
	$(CC) $(CFLAGS) $(OBJ) $(LDFLAGS) -o $(BIN)

%.o: %.c
	$(CC) $(CFLAGS) -c $< -o $@

install: $(BIN)
	mkdir -p $(DESTDIR)$(BINDIR)
	install -m 755 $(BIN) $(DESTDIR)$(BINDIR)/$(BIN)

clean:
	rm -f $(OBJ) $(BIN)

.PHONY: all clean install
