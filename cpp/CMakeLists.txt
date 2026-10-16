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

# Realtime's UDP channel uses raw sockets — Winsock on Windows.
if(WIN32)
  target_link_libraries(multiplayer_sdk PUBLIC ws2_32)
endif()

# Set C++ standard
target_compile_features(multiplayer_sdk PUBLIC cxx_std_17)

# Example executable
add_executable(game_example
  examples/Game.cpp
)

target_link_libraries(game_example
  PRIVATE multiplayer_sdk
)

# Optional Steam support: enables the Steam login demo in Game.cpp via
# include/michitai/multiplayer/Steam/SteamTicket.h (GetAuthTicketForWebApi
# -> Players::registerPlayerWithSteam). Requires the Steamworks SDK
# (>= 1.57, partner.steamgames.com) — point STEAMWORKS_SDK_DIR at the
# extracted sdk/ dir containing public/ and redistributable_bin/.
# Ship steam_api64.dll + steam_appid.txt next to the exe.
#
# option(MICHITAI_STEAM "Build the Steam login demo" OFF)
# if(MICHITAI_STEAM)
#   set(STEAMWORKS_SDK_DIR "" CACHE PATH "Path to Steamworks SDK root")
#   target_compile_definitions(game_example PRIVATE MICHITAI_STEAM)
#   target_include_directories(game_example PRIVATE "${STEAMWORKS_SDK_DIR}/public")
#   if(WIN32)
#     target_link_directories(game_example PRIVATE "${STEAMWORKS_SDK_DIR}/redistributable_bin/win64")
#     target_link_libraries(game_example PRIVATE steam_api64)
#   elseif(APPLE)
#     target_link_directories(game_example PRIVATE "${STEAMWORKS_SDK_DIR}/redistributable_bin/osx")
#     target_link_libraries(game_example PRIVATE steam_api)
#   else()
#     target_link_directories(game_example PRIVATE "${STEAMWORKS_SDK_DIR}/redistributable_bin/linux64")
#     target_link_libraries(game_example PRIVATE steam_api)
#   endif()
# endif()
