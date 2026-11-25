# make release VERSION=2.2.0
.DEFAULT_GOAL := help

SHELL := /usr/bin/bash
.SHELLFLAGS := -eu -o pipefail -c

APP_NAME := CyberCalc.NET9
VERSION_FILE := version.txt
REMOTE := origin

.PHONY: help release rerun check-version check-clean

help:
	@echo ""
	@echo "  $(APP_NAME) - Release Management"
	@echo ""
	@echo "  make help                  Show available commands"
	@echo "  make check-version         Display current version"
	@echo "  make check-clean           Check Git working tree"
	@echo "  make release VERSION=x.y.z Publish a release from pushed main"
	@echo "  make rerun VERSION=x.y.z   Re-run the existing release workflow"
	@echo ""

check-version:
	@test -f "$(VERSION_FILE)" || { echo "ERROR: $(VERSION_FILE) not found"; exit 1; }
	@echo "Current version: $$(cat $(VERSION_FILE))"

check-clean:
	@git diff --quiet && git diff --cached --quiet && test -z "$$(git ls-files --others --exclude-standard)" \
		&& echo "Working tree is clean" \
		|| { echo "ERROR: Working tree contains uncommitted changes"; exit 1; }


release:
	@test -n "$(VERSION)" || { echo "ERROR: Specify VERSION=x.y.z"; exit 1; }
	@echo "$(VERSION)" | grep -Eq '^[0-9]+\.[0-9]+\.[0-9]+$$' || { echo "ERROR: Invalid version"; exit 1; }
	@test -f "$(VERSION_FILE)" || { echo "ERROR: version.txt not found"; exit 1; }
	@current=$$(tr -d '\r\n' < "$(VERSION_FILE)"); \
	if [ "$$current" != "$(VERSION)" ]; then \
		echo "ERROR: version.txt must contain $(VERSION)"; exit 1; \
	fi; \
	branch=$$(git branch --show-current); \
	test "$$branch" = "main" || { echo "ERROR: Switch to main before release"; exit 1; }; \
	git diff --quiet && git diff --cached --quiet && \
	test -z "$$(git ls-files --others --exclude-standard)" || \
		{ echo "ERROR: Working tree is not clean"; exit 1; }; \
	git remote get-url $(REMOTE) >/dev/null 2>&1 || \
		{ echo "ERROR: Remote not configured"; exit 1; }; \
	git pull --ff-only $(REMOTE) main || { echo "ERROR: Could not synchronize main"; exit 1; }; \
	tag="v$(VERSION)"; \
	git rev-parse -q --verify "refs/tags/$$tag" >/dev/null && \
		{ echo "ERROR: Local tag $$tag already exists"; exit 1; } || true; \
	if git ls-remote --exit-code --tags $(REMOTE) "refs/tags/$$tag" >/dev/null 2>&1; then \
		echo "ERROR: Remote tag $$tag already exists"; exit 1; \
	fi; \
	echo "Creating release tag $$tag at $$(git rev-parse --short HEAD)"; \
	git tag -a "$$tag" -m "$(APP_NAME) $(VERSION)"; \
	git push $(REMOTE) "$$tag"; \
	echo "Release $$tag pushed successfully"

rerun:
	@test -n "$(VERSION)" || { echo "ERROR: Specify VERSION=x.y.z"; exit 1; }
	@echo "$(VERSION)" | grep -Eq '^[0-9]+\.[0-9]+\.[0-9]+$$' || { echo "ERROR: Invalid version"; exit 1; }
	@test -f "$(VERSION_FILE)" || { echo "ERROR: version.txt not found"; exit 1; }
	@current=$$(tr -d '\r\n' < "$(VERSION_FILE)"); \
	if [ "$$current" != "$(VERSION)" ]; then \
		echo "ERROR: version.txt must contain $(VERSION)"; exit 1; \
	fi; \
	tag="v$(VERSION)"; \
	git rev-parse -q --verify "refs/tags/$$tag" >/dev/null || \
		{ echo "ERROR: Local tag $$tag does not exist"; exit 1; }; \
	if ! git ls-remote --exit-code --tags $(REMOTE) "refs/tags/$$tag" >/dev/null 2>&1; then \
		echo "ERROR: Remote tag $$tag does not exist"; exit 1; \
	fi; \
	command -v gh >/dev/null 2>&1 || { echo "ERROR: GitHub CLI 'gh' is not installed"; exit 1; }; \
	gh auth status >/dev/null 2>&1 || { echo "ERROR: GitHub CLI is not authenticated"; exit 1; }; \
	echo "Dispatching existing release workflow for $$tag"; \
	gh workflow run release.yml --ref "$$tag" -f tag="$$tag"; \
	echo "Release workflow dispatched for $$tag"

