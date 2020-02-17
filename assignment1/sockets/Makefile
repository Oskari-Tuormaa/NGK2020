CXX=c++
CXXFLAGS=-I.

BUILDDIR=build
BINDIR=bin

EXE=client/file_client server/file_server
EXE:=$(addprefix $(BINDIR)/,$(EXE))
SOURCE=functions/sockfunctions.cpp

SOURCE_CLIENT=file_client/main.cpp $(SOURCE)
OBJ_CLIENT=$(addprefix $(BUILDDIR)/,$(SOURCE_CLIENT:.cpp=.o))
DEP_CLIENT=$(addprefix $(BUILDDIR)/,$(SOURCE_CLIENT:.cpp=.d))

SOURCE_SERVER=file_server/main.cpp $(SOURCE)
OBJ_SERVER=$(addprefix $(BUILDDIR)/,$(SOURCE_SERVER:.cpp=.o))
DEP_SERVER=$(addprefix $(BUILDDIR)/,$(SOURCE_SERVER:.cpp=.d))

all: $(EXE)

$(BINDIR)/client/file_client: $(DEP_CLIENT) $(OBJ_CLIENT)
	mkdir -p $(dir $@)
	$(CXX) $(CXXFLAGS) -o $@ $(OBJ_CLIENT)

$(BINDIR)/server/file_server: $(DEP_SERVER) $(OBJ_SERVER)
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
