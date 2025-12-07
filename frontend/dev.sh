#!/bin/bash

# Development script that watches md-ui library and serves configurator
# This ensures md-ui is always built when changes are made

echo "🚀 Starting development environment..."
echo ""

# Build md-ui once first
echo "📦 Building md-ui library..."
npm run build-md-ui

if [ $? -ne 0 ]; then
    echo "❌ Initial md-ui build failed!"
    exit 1
fi

echo "✅ Initial md-ui build complete"
echo ""

# Start watching md-ui in the background
echo "👀 Starting md-ui watch mode..."
npm run watch-md-ui &
MD_UI_PID=$!

# Give it a moment to start
sleep 2

# Start the configurator dev server
echo "🌐 Starting configurator dev server on port 8192..."
npm run ng serve configurator -- --port 8192

# When configurator stops (Ctrl+C), also stop md-ui watch
echo ""
echo "🛑 Stopping development environment..."
kill $MD_UI_PID 2>/dev/null

echo "✅ Done!"
