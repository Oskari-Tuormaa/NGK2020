CXX=c++
CXXFLAGS=-I.

BUILDDIR=build
BINDIR=bin

EXE=client server
EXE:=$(addprefix $(BINDIR)/,$(EXE))
SOURCE=#udp_comm/udp_comm.hpp

SOURCE_CLIENT=client.cpp $(SOURCE)
OBJ_CLIENT=$(addprefix $(BUILDDIR)/,$(SOURCE_CLIENT:.cpp=.o))
DEP_CLIENT=$(addprefix $(BUILDDIR)/,$(SOURCE_CLIENT:.cpp=.d))

SOURCE_SERVER=server.cpp $(SOURCE)
OBJ_SERVER=$(addprefix $(BUILDDIR)/,$(SOURCE_SERVER:.cpp=.o))
DEP_SERVER=$(addprefix $(BUILDDIR)/,$(SOURCE_SERVER:.cpp=.d))

all: $(EXE)

$(BINDIR)/client: $(DEP_CLIENT) $(OBJ_CLIENT)
	mkdir -p $(dir $@)
	$(CXX) $(CXXFLAGS) -o $@ $(OBJ_CLIENT)

$(BINDIR)/server: $(DEP_SERVER) $(OBJ_SERVER)
	mkdir -p $(dir $@)
	$(CXX) $(CXXFLAGS) -o $@ $(OBJ_SERVER)

$(BUILDDIR)/%.d: %.cpp
	mkdir -p $(dir $@)
	$(CXX) -MT$@ -MM $(CXXFLAGS) $< > $@
	$(CXX) -MT$(@:.d=.o) -MM $(CXXFLAGS) $< >> $@

$(BUILDDIR)/%.o: %.cpp
	mkdir -p $(dir $@)
	$(CXX) -c $< -o $@ $(CXXFLAGS)

clean:
	rm -rf $(BUILDDIR) $(BINDIR)

.PHONY: clean all

ifneq ($(MAKECMDGOALS),clean)
-include $(DEPS)
endif
