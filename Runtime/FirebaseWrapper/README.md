# Firebase Wrappers - Conditional Compilation

This folder contains wrapper classes for Firebase services that use conditional compilation to ensure they only compile when their respective Firebase packages are available.

## How It Works

Each Firebase wrapper file is conditionally compiled based on specific scripting define symbols:

- **FirebaseAuthWrapper.cs** → Requires `FIREBASE_AUTH_AVAILABLE` symbol
- **FirebaseDatabaseWrapper.cs** → Requires `FIREBASE_DATABASE_AVAILABLE` symbol  
- **FirebaseCloudFunctionWrapper.cs** → Requires `FIREBASE_FUNCTIONS_AVAILABLE` symbol

## Automatic Package Detection

The `FirebasePackageDetector` editor script automatically detects which Firebase packages are installed and adds/removes the appropriate scripting define symbols. This runs:

- **Automatically** when Unity loads
- **Manually** via menu: `Tools → Jambav → Update Firebase Package Symbols`

## Manual Setup (Optional)

If you need to manually configure the symbols:

1. Go to **Edit → Project Settings → Player**
2. Expand **Other Settings** section
3. Find **Scripting Define Symbols**
4. Add the symbols for the Firebase packages you have installed:
   - `FIREBASE_AUTH_AVAILABLE` (if Firebase Auth is installed)
   - `FIREBASE_DATABASE_AVAILABLE` (if Firebase Database is installed)
   - `FIREBASE_FUNCTIONS_AVAILABLE` (if Firebase Functions is installed)

## Benefits

- **No compilation errors** when Firebase packages are missing
- **Modular** - Install only the Firebase packages you need
- **WebGL support** - Separate implementation paths for WebGL vs native platforms
- **Clean project** - Unused wrapper code is excluded from builds

## Troubleshooting

If you're getting compilation errors:

1. Run `Tools → Jambav → Update Firebase Package Symbols` from the menu
2. Check that the required Firebase package is properly installed
3. Verify the scripting define symbols in Player Settings match your installed packages
4. Restart Unity if symbols don't update immediately

## Platform Support

- **WebGL**: Uses JavaScript interop (DllImport)
- **Other Platforms**: Uses native Firebase SDK (when `FIREBASE_*_AVAILABLE` symbols are defined)

