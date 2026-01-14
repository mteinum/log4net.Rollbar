#!/bin/bash

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if version argument is provided
if [ -z "$1" ]; then
    echo -e "${RED}Error: Version number is required${NC}"
    echo "Usage: ./release.sh <version>"
    echo "Example: ./release.sh 1.2.0"
    exit 1
fi

VERSION=$1
TAG="v${VERSION}"

# Validate version format (semantic versioning)
if ! [[ $VERSION =~ ^[0-9]+\.[0-9]+\.[0-9]+(-[a-zA-Z0-9]+)?$ ]]; then
    echo -e "${RED}Error: Invalid version format${NC}"
    echo "Version should follow semantic versioning: MAJOR.MINOR.PATCH"
    echo "Examples: 1.2.0, 1.2.3-beta, 2.0.0-rc1"
    exit 1
fi

# Check if tag already exists
if git rev-parse "$TAG" >/dev/null 2>&1; then
    echo -e "${RED}Error: Tag $TAG already exists${NC}"
    exit 1
fi

# Check if there are uncommitted changes
if ! git diff-index --quiet HEAD --; then
    echo -e "${YELLOW}Warning: You have uncommitted changes${NC}"
    read -p "Do you want to continue? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Release cancelled"
        exit 1
    fi
fi

# Get current branch
CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
echo -e "${GREEN}Current branch: $CURRENT_BRANCH${NC}"

# Confirm release
echo -e "\n${YELLOW}Release Summary:${NC}"
echo "  Version: $VERSION"
echo "  Tag: $TAG"
echo "  Branch: $CURRENT_BRANCH"
echo ""
read -p "Create and push this release? (y/n) " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Release cancelled"
    exit 1
fi

# Create the tag
echo -e "\n${GREEN}Creating tag $TAG...${NC}"
git tag -a "$TAG" -m "Release version $VERSION"

# Push the tag
echo -e "${GREEN}Pushing tag to remote...${NC}"
git push origin "$TAG"

echo -e "\n${GREEN}✓ Release $TAG created and pushed successfully!${NC}"
echo -e "\nThe GitHub Actions workflow will now:"
echo "  1. Build the project"
echo "  2. Run tests"
echo "  3. Create NuGet package"
echo "  4. Publish to NuGet.org"
echo ""
echo -e "Monitor the release at: ${GREEN}https://github.com/mteinum/log4net.Rollbar/actions${NC}"
echo -e "View releases at: ${GREEN}https://github.com/mteinum/log4net.Rollbar/releases${NC}"
