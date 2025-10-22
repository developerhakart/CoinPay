# CoinPay Documentation

This directory contains the DocFX documentation site for the CoinPay API.

## Quick Start

### Build Documentation
```bash
cd docfx
docfx build
```

### Serve Documentation Locally
```bash
docfx serve _site
```

The documentation will be available at: http://localhost:8080

## What's Included

- **Getting Started Guide** - Installation and setup instructions
- **API Usage Examples** - Practical examples with curl commands
- **Configuration Guide** - Configuration options and database setup
- **API Reference** - Complete API endpoint documentation

## Files

- `docfx.json` - DocFX configuration file
- `index.md` - Homepage
- `toc.yml` - Table of contents
- `articles/` - Documentation articles
  - `getting-started.md` - Getting started guide
  - `api-examples.md` - API usage examples
  - `configuration.md` - Configuration guide
- `api/` - API reference documentation
  - `index.md` - API overview
- `_site/` - Generated documentation (excluded from git)

## Building from Scratch

If you need to rebuild everything:

```bash
# Clean previous build
rm -rf _site api obj

# Build documentation
docfx build

# Serve locally
docfx serve _site
```

## Continuous Documentation

For auto-rebuild on file changes:

```bash
docfx build --serve
```

This will watch for changes and automatically rebuild the documentation.

## Deployment

The documentation can be deployed to:
- GitHub Pages
- Azure Static Web Apps
- Any static hosting service

Just deploy the contents of the `_site` directory.

## More Information

- [DocFX Documentation](https://dotnet.github.io/docfx/)
- [DocFX GitHub](https://github.com/dotnet/docfx)
