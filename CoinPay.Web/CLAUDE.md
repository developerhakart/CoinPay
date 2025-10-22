# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CoinPay.Web is a React-based web application for cryptocurrency payment processing. The project uses TypeScript, Vite as the build tool, and Tailwind CSS for styling.

## Development Commands

### Running the Development Server
```bash
npm run dev
```
Starts the Vite development server on **port 3000** at http://localhost:3000

### Building for Production
```bash
npm run build
```
TypeScript compilation runs first (`tsc`), followed by Vite build. Build output goes to `dist/`.

### Linting
```bash
npm run lint
```
Runs ESLint on TypeScript and TSX files with strict warnings (max-warnings 0).

### Preview Production Build
```bash
npm run preview
```
Serves the production build locally for testing.

## Architecture

### Tech Stack
- **Framework**: React 18 with TypeScript
- **Build Tool**: Vite 5
- **Styling**: Tailwind CSS 3 with PostCSS and Autoprefixer
- **Linting**: ESLint with TypeScript parser and React-specific plugins

### Project Structure
```
CoinPay.Web/
├── src/              # Application source code
│   ├── App.tsx      # Root application component
│   ├── main.tsx     # Application entry point, React root mounting
│   └── index.css    # Global styles, Tailwind directives
├── index.html       # HTML entry point, references /src/main.tsx
└── vite.config.ts   # Vite configuration (port 3000)
```

### Configuration Files

**TypeScript Configuration**
- `tsconfig.json`: Main TypeScript config with strict mode enabled, targets ES2020, uses modern bundler resolution
- `tsconfig.node.json`: Separate config for Vite configuration files

**Build Configuration**
- `vite.config.ts`: Vite setup with React plugin, configured for port 3000
- Development server uses HMR (Hot Module Replacement) for fast refresh

**Styling Configuration**
- `tailwind.config.js`: Tailwind scans `index.html` and all files in `src/**/*.{js,ts,jsx,tsx}`
- `postcss.config.js`: PostCSS with Tailwind and Autoprefixer plugins

### TypeScript Settings
- Strict mode enabled for type safety
- Unused locals/parameters flagged as errors
- Modern module resolution ("bundler")
- JSX transform: `react-jsx` (no need to import React in components)

## Development Notes

### Port Configuration
The development server is hardcoded to run on port 3000 in `vite.config.ts:8`. Change this if you need a different port.

### Adding New Components
Components should be placed in `src/` directory. The project uses React functional components with hooks and TypeScript strict typing.

### Styling Approach
Tailwind utility classes are used throughout. Custom styles can be added to `src/index.css` using Tailwind's `@layer` directive if needed.

### Module Resolution
TypeScript is configured with "bundler" module resolution, which is optimized for Vite. Import paths should not include file extensions for TypeScript files (Vite handles this).
