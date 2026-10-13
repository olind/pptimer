#!/bin/sh
# Builds build/PPTimer.app, a self-contained macOS menu bar app.
# Usage: tools/package-macos.sh [osx-arm64|osx-x64]   (default: osx-arm64, Apple Silicon)
set -eu

RID="${1:-osx-arm64}"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
BUILD="$ROOT/build"
APP="$BUILD/PPTimer.app"
VERSION="2.0.0"

rm -rf "$APP" "$BUILD/publish-$RID"
dotnet publish "$ROOT/PPTimer" -c Release -r "$RID" --self-contained -o "$BUILD/publish-$RID"

mkdir -p "$APP/Contents/MacOS" "$APP/Contents/Resources"
cp -R "$BUILD/publish-$RID/." "$APP/Contents/MacOS/"
cp "$ROOT/PPTimer/Assets/PPTimer.icns" "$APP/Contents/Resources/"

cat > "$APP/Contents/Info.plist" <<EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleName</key><string>PPTimer</string>
  <key>CFBundleDisplayName</key><string>PPTimer</string>
  <key>CFBundleIdentifier</key><string>com.github.olind.pptimer</string>
  <key>CFBundleExecutable</key><string>PPTimer</string>
  <key>CFBundleIconFile</key><string>PPTimer</string>
  <key>CFBundlePackageType</key><string>APPL</string>
  <key>CFBundleVersion</key><string>$VERSION</string>
  <key>CFBundleShortVersionString</key><string>$VERSION</string>
  <key>LSMinimumSystemVersion</key><string>12.0</string>
  <key>LSUIElement</key><true/>
  <key>NSHighResolutionCapable</key><true/>
</dict>
</plist>
EOF

# Ad-hoc signature so macOS will run it locally (not notarized).
codesign --force --deep --sign - "$APP"
echo "Built $APP"
