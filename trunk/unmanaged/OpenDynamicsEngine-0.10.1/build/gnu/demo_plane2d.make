# C++ Console Executable Makefile autogenerated by premake
# Don't edit this file! Instead edit `premake.lua` then rerun `make`

ifndef CONFIG
  CONFIG=DebugSingleDLL
endif

# if multiple archs are defined turn off automated dependency generation
DEPFLAGS := $(if $(word 2, $(TARGET_ARCH)), , -MMD)

ifeq ($(CONFIG),DebugSingleDLL)
  BINDIR := ../../lib/DebugSingleDLL
  LIBDIR := ../../lib/DebugSingleDLL
  OBJDIR := obj/plane2d/DebugSingleDLL
  OUTDIR := ../../lib/DebugSingleDLL
  CPPFLAGS := $(DEPFLAGS) -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dSINGLE" -I "../../include" -I "../../ode/src"
  CFLAGS += $(CPPFLAGS) $(TARGET_ARCH) -g
  CXXFLAGS += $(CFLAGS)
  LDFLAGS += -L$(BINDIR) -L$(LIBDIR) -lode_singled -ldrawstuffd -luser32 -lwinmm -lgdi32 -lopengl32 -lglu32
  LDDEPS := ../../lib/DebugSingleDLL/ode_singled.dll ../../lib/DebugSingleDLL/drawstuffd.dll
  RESFLAGS := -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dSINGLE" -I "../../include" -I "../../ode/src"
  TARGET := demo_plane2d.exe
 BLDCMD = $(CXX) -o $(OUTDIR)/$(TARGET) $(OBJECTS) $(LDFLAGS) $(RESOURCES) $(TARGET_ARCH)
endif

ifeq ($(CONFIG),ReleaseSingleDLL)
  BINDIR := ../../lib/ReleaseSingleDLL
  LIBDIR := ../../lib/ReleaseSingleDLL
  OBJDIR := obj/plane2d/ReleaseSingleDLL
  OUTDIR := ../../lib/ReleaseSingleDLL
  CPPFLAGS := $(DEPFLAGS) -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dSINGLE" -I "../../include" -I "../../ode/src"
  CFLAGS += $(CPPFLAGS) $(TARGET_ARCH) -g
  CXXFLAGS += $(CFLAGS)
  LDFLAGS += -L$(BINDIR) -L$(LIBDIR) -lode_single -ldrawstuff -luser32 -lwinmm -lgdi32 -lopengl32 -lglu32
  LDDEPS := ../../lib/ReleaseSingleDLL/ode_single.dll ../../lib/ReleaseSingleDLL/drawstuff.dll
  RESFLAGS := -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dSINGLE" -I "../../include" -I "../../ode/src"
  TARGET := demo_plane2d.exe
 BLDCMD = $(CXX) -o $(OUTDIR)/$(TARGET) $(OBJECTS) $(LDFLAGS) $(RESOURCES) $(TARGET_ARCH)
endif

ifeq ($(CONFIG),DebugSingleLib)
  BINDIR := ../../lib/DebugSingleLib
  LIBDIR := ../../lib/DebugSingleLib
  OBJDIR := obj/plane2d/DebugSingleLib
  OUTDIR := ../../lib/DebugSingleLib
  CPPFLAGS := $(DEPFLAGS) -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dSINGLE" -I "../../include" -I "../../ode/src"
  CFLAGS += $(CPPFLAGS) $(TARGET_ARCH) -g
  CXXFLAGS += $(CFLAGS)
  LDFLAGS += -L$(BINDIR) -L$(LIBDIR) -lode_singled -ldrawstuffd -luser32 -lwinmm -lgdi32 -lopengl32 -lglu32
  LDDEPS := ../../lib/DebugSingleLib/libode_singled.a ../../lib/DebugSingleLib/libdrawstuffd.a
  RESFLAGS := -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dSINGLE" -I "../../include" -I "../../ode/src"
  TARGET := demo_plane2d.exe
 BLDCMD = $(CXX) -o $(OUTDIR)/$(TARGET) $(OBJECTS) $(LDFLAGS) $(RESOURCES) $(TARGET_ARCH)
endif

ifeq ($(CONFIG),ReleaseSingleLib)
  BINDIR := ../../lib/ReleaseSingleLib
  LIBDIR := ../../lib/ReleaseSingleLib
  OBJDIR := obj/plane2d/ReleaseSingleLib
  OUTDIR := ../../lib/ReleaseSingleLib
  CPPFLAGS := $(DEPFLAGS) -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dSINGLE" -I "../../include" -I "../../ode/src"
  CFLAGS += $(CPPFLAGS) $(TARGET_ARCH) -g
  CXXFLAGS += $(CFLAGS)
  LDFLAGS += -L$(BINDIR) -L$(LIBDIR) -lode_single -ldrawstuff -luser32 -lwinmm -lgdi32 -lopengl32 -lglu32
  LDDEPS := ../../lib/ReleaseSingleLib/libode_single.a ../../lib/ReleaseSingleLib/libdrawstuff.a
  RESFLAGS := -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dSINGLE" -I "../../include" -I "../../ode/src"
  TARGET := demo_plane2d.exe
 BLDCMD = $(CXX) -o $(OUTDIR)/$(TARGET) $(OBJECTS) $(LDFLAGS) $(RESOURCES) $(TARGET_ARCH)
endif

ifeq ($(CONFIG),DebugDoubleDLL)
  BINDIR := ../../lib/DebugDoubleDLL
  LIBDIR := ../../lib/DebugDoubleDLL
  OBJDIR := obj/plane2d/DebugDoubleDLL
  OUTDIR := ../../lib/DebugDoubleDLL
  CPPFLAGS := $(DEPFLAGS) -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dDOUBLE" -I "../../include" -I "../../ode/src"
  CFLAGS += $(CPPFLAGS) $(TARGET_ARCH) -g
  CXXFLAGS += $(CFLAGS)
  LDFLAGS += -L$(BINDIR) -L$(LIBDIR) -lode_doubled -ldrawstuffd -luser32 -lwinmm -lgdi32 -lopengl32 -lglu32
  LDDEPS := ../../lib/DebugDoubleDLL/ode_doubled.dll ../../lib/DebugDoubleDLL/drawstuffd.dll
  RESFLAGS := -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dDOUBLE" -I "../../include" -I "../../ode/src"
  TARGET := demo_plane2d.exe
 BLDCMD = $(CXX) -o $(OUTDIR)/$(TARGET) $(OBJECTS) $(LDFLAGS) $(RESOURCES) $(TARGET_ARCH)
endif

ifeq ($(CONFIG),ReleaseDoubleDLL)
  BINDIR := ../../lib/ReleaseDoubleDLL
  LIBDIR := ../../lib/ReleaseDoubleDLL
  OBJDIR := obj/plane2d/ReleaseDoubleDLL
  OUTDIR := ../../lib/ReleaseDoubleDLL
  CPPFLAGS := $(DEPFLAGS) -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dDOUBLE" -I "../../include" -I "../../ode/src"
  CFLAGS += $(CPPFLAGS) $(TARGET_ARCH) -g
  CXXFLAGS += $(CFLAGS)
  LDFLAGS += -L$(BINDIR) -L$(LIBDIR) -lode_double -ldrawstuff -luser32 -lwinmm -lgdi32 -lopengl32 -lglu32
  LDDEPS := ../../lib/ReleaseDoubleDLL/ode_double.dll ../../lib/ReleaseDoubleDLL/drawstuff.dll
  RESFLAGS := -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dDOUBLE" -I "../../include" -I "../../ode/src"
  TARGET := demo_plane2d.exe
 BLDCMD = $(CXX) -o $(OUTDIR)/$(TARGET) $(OBJECTS) $(LDFLAGS) $(RESOURCES) $(TARGET_ARCH)
endif

ifeq ($(CONFIG),DebugDoubleLib)
  BINDIR := ../../lib/DebugDoubleLib
  LIBDIR := ../../lib/DebugDoubleLib
  OBJDIR := obj/plane2d/DebugDoubleLib
  OUTDIR := ../../lib/DebugDoubleLib
  CPPFLAGS := $(DEPFLAGS) -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dDOUBLE" -I "../../include" -I "../../ode/src"
  CFLAGS += $(CPPFLAGS) $(TARGET_ARCH) -g
  CXXFLAGS += $(CFLAGS)
  LDFLAGS += -L$(BINDIR) -L$(LIBDIR) -lode_doubled -ldrawstuffd -luser32 -lwinmm -lgdi32 -lopengl32 -lglu32
  LDDEPS := ../../lib/DebugDoubleLib/libode_doubled.a ../../lib/DebugDoubleLib/libdrawstuffd.a
  RESFLAGS := -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dDOUBLE" -I "../../include" -I "../../ode/src"
  TARGET := demo_plane2d.exe
 BLDCMD = $(CXX) -o $(OUTDIR)/$(TARGET) $(OBJECTS) $(LDFLAGS) $(RESOURCES) $(TARGET_ARCH)
endif

ifeq ($(CONFIG),ReleaseDoubleLib)
  BINDIR := ../../lib/ReleaseDoubleLib
  LIBDIR := ../../lib/ReleaseDoubleLib
  OBJDIR := obj/plane2d/ReleaseDoubleLib
  OUTDIR := ../../lib/ReleaseDoubleLib
  CPPFLAGS := $(DEPFLAGS) -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dDOUBLE" -I "../../include" -I "../../ode/src"
  CFLAGS += $(CPPFLAGS) $(TARGET_ARCH) -g
  CXXFLAGS += $(CFLAGS)
  LDFLAGS += -L$(BINDIR) -L$(LIBDIR) -lode_double -ldrawstuff -luser32 -lwinmm -lgdi32 -lopengl32 -lglu32
  LDDEPS := ../../lib/ReleaseDoubleLib/libode_double.a ../../lib/ReleaseDoubleLib/libdrawstuff.a
  RESFLAGS := -D "_CRT_SECURE_NO_DEPRECATE" -D "WIN32" -D "dDOUBLE" -I "../../include" -I "../../ode/src"
  TARGET := demo_plane2d.exe
 BLDCMD = $(CXX) -o $(OUTDIR)/$(TARGET) $(OBJECTS) $(LDFLAGS) $(RESOURCES) $(TARGET_ARCH)
endif

OBJECTS := \
	$(OBJDIR)/demo_plane2d.o \

RESOURCES := \
	$(OBJDIR)/resources.res \

MKDIR_TYPE := msdos
CMD := $(subst \,\\,$(ComSpec)$(COMSPEC))
ifeq (,$(CMD))
  MKDIR_TYPE := posix
endif
ifeq (/bin,$(findstring /bin,$(SHELL)))
  MKDIR_TYPE := posix
endif
ifeq ($(MKDIR_TYPE),posix)
  CMD_MKBINDIR := mkdir -p $(BINDIR)
  CMD_MKLIBDIR := mkdir -p $(LIBDIR)
  CMD_MKOUTDIR := mkdir -p $(OUTDIR)
  CMD_MKOBJDIR := mkdir -p $(OBJDIR)
else
  CMD_MKBINDIR := $(CMD) /c if not exist $(subst /,\\,$(BINDIR)) mkdir $(subst /,\\,$(BINDIR))
  CMD_MKLIBDIR := $(CMD) /c if not exist $(subst /,\\,$(LIBDIR)) mkdir $(subst /,\\,$(LIBDIR))
  CMD_MKOUTDIR := $(CMD) /c if not exist $(subst /,\\,$(OUTDIR)) mkdir $(subst /,\\,$(OUTDIR))
  CMD_MKOBJDIR := $(CMD) /c if not exist $(subst /,\\,$(OBJDIR)) mkdir $(subst /,\\,$(OBJDIR))
endif

.PHONY: clean

$(OUTDIR)/$(TARGET): $(OBJECTS) $(LDDEPS) $(RESOURCES)
	@echo Linking demo_plane2d
	-@$(CMD_MKBINDIR)
	-@$(CMD_MKLIBDIR)
	-@$(CMD_MKOUTDIR)
	@$(BLDCMD)

clean:
	@echo Cleaning demo_plane2d
ifeq ($(MKDIR_TYPE),posix)
	-@rm -f $(OUTDIR)/$(TARGET)
	-@rm -rf $(OBJDIR)
else
	-@if exist $(subst /,\,$(OUTDIR)/$(TARGET)) del /q $(subst /,\,$(OUTDIR)/$(TARGET))
	-@if exist $(subst /,\,$(OBJDIR)) del /q $(subst /,\,$(OBJDIR))
	-@if exist $(subst /,\,$(OBJDIR)) rmdir /s /q $(subst /,\,$(OBJDIR))
endif

$(OBJDIR)/demo_plane2d.o: ../../ode/demo/demo_plane2d.cpp
	-@$(CMD_MKOBJDIR)
	@echo $(notdir $<)
	@$(CXX) $(CXXFLAGS) -o "$@" -c "$<"

$(OBJDIR)/resources.res: ../../drawstuff/src/resources.rc
	-@$(CMD_MKOBJDIR)
	@echo $(notdir $<)
	@windres $< -O coff -o "$@" $(RESFLAGS)

-include $(OBJECTS:%.o=%.d)

