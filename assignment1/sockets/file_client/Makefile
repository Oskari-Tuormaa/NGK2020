SOURCES=main.cpp
OBJECTS=$(addprefix $(BUILD_DIR)/,$(SOURCES:.cpp=.o))
DEPS=$(addprefix $(BUILD_DIR)/,$(SOURCES:.cpp=.d))
EXE=file_client
CXX=g++
CXXFLAGS=-I.

BUILD_DIR=build
BIN_DIR=bin

# Linker
$(BIN_DIR)/$(EXE): $(DEPS) $(OBJECTS) # << Check the $(DEPS) new dependency
	mkdir -p $(BIN_DIR)
	$(CXX) $(CXXFLAGS) -o $@ $(OBJECTS)

# O-filer
$(BUILD_DIR)/%.o: %.cpp
	$(CXX) -c $< -o $@ $(CXXFLAGS)

# Rule that describes how a .d (dependency) file is created from a .cpp file
# Similar to the assigment that you just completed %.cpp -> %.o
$(BUILD_DIR)/%.d: %.cpp
	mkdir -p $(BUILD_DIR)
	$(CXX) -MT$@ -MM $(CXXFLAGS) $< > $@
	$(CXX) -MT$(@:.d=.o) -MM $(CXXFLAGS) $< >> $@

format: $(SOURCES:.cpp=.format)
%.format: %.cpp
	@echo "Formatting file '$<'"...
	@clang-format -i $<
	@echo "" > $@

.PHONY: clean
clean:
	rm -rf bin/ build/ *.format

ifneq ($(MAKECMDGOALS),clean)
-include $(DEPS)
endif

