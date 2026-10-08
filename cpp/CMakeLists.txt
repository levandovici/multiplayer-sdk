cmake_minimum_required(VERSION 3.20)
project(multiplayer_cpp_sdk LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

include(FetchContent)

# nlohmann_json (header-only)
FetchContent_Declare(
  nlohmann_json
  GIT_REPOSITORY https://github.com/nlohmann/json.git
  GIT_TAG v3.11.3
)
FetchContent_MakeAvailable(nlohmann_json)

# cpr (HTTP, uses libcurl)
# Note: On Windows, CPR will fetch/build libcurl automatically via CMake.
FetchContent_Declare(
  cpr
  GIT_REPOSITORY https://github.com/libcpr/cpr.git
  GIT_TAG 1.12.0
)
set(CPR_USE_SYSTEM_CURL OFF)
# Realtime.h uses libcurl's WebSocket API — the bundled curl must keep the
# ws/wss protocols (cpr's HTTP_ONLY default would strip them).
set(CPR_ENABLE_CURL_HTTP_ONLY OFF CACHE BOOL "" FORCE)
set(ENABLE_WEBSOCKETS ON CACHE BOOL "" FORCE)   # curl < 8.11 needed this opt-in
FetchContent_MakeAvailable(cpr)

# SDK library
add_library(multiplayer_sdk STATIC
  src/Client.cpp
)

# Public headers
target_include_directories(multiplayer_sdk
  PUBLIC ${CMAKE_CURRENT_SOURCE_DIR}/include
)

# Link dependencies
target_link_libraries(multiplayer_sdk
  PUBLIC nlohmann_json::nlohmann_json
  PUBLIC cpr::cpr
)

# Realtime.h calls libcurl's WS API directly (curl_ws_send/recv) — link the
# curl target whether cpr bundled it or found a system one.
if(TARGET libcurl)
  target_link_libraries(multiplayer_sdk PUBLIC libcurl)
elseif(TARGET CURL::libcurl)
  target_link_libraries(multiplayer_sdk PUBLIC CURL::libcurl)
else()
  find_package(CURL REQUIRED)
  target_link_libraries(multiplayer_sdk PUBLIC CURL::libcurl)
endif()

# Threads for the realtime receive/heartbeat loops
find_package(Threads REQUIRED)
target_link_libraries(multiplayer_sdk PUBLIC Threads::Threads)

# Set C++ standard
target_compile_features(multiplayer_sdk PUBLIC cxx_std_17)

# Example executable
add_executable(game_example
  examples/Game.cpp
)

target_link_libraries(game_example
  PRIVATE multiplayer_sdk
)
