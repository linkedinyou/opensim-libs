noinst_PROGRAMS=SoftDemo

SoftDemo_SOURCES=SoftDemo.cpp SoftDemo.h main.cpp
SoftDemo_CXXFLAGS=-I@top_builddir@/src -I@top_builddir@/Demos/OpenGL $(CXXFLAGS)
SoftDemo_LDADD=-L../OpenGL -lbulletopenglsupport -L../../src -lbulletsoftbody -lbulletdynamics -lbulletcollision -lbulletmath @opengl_LIBS@
