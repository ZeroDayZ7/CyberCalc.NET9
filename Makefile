# make release VERSION=2.1.10
.DEFAULT_GOAL := help

SHELL := /usr/bin/bash
.SHELLFLAGS := -eu -o pipefail -c

APP_NAME := CyberCalc.NET9
VERSION_FILE := version.txt
REMOTE := origin

.PHONY: help release check-version check-clean

help:
	@echo ""
	@echo "  $(APP_NAME) - Release Management"
	@echo ""
	@echo "  make help                  Show available commands"
	@echo "  make check-version         Display current version"
	@echo "  make check-clean           Check Git working tree"
	@echo "  make release VERSION=x.y.z Publish a new release"
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
	if [ "$$current" = "$(VERSION)" ]; then \
		echo "ERROR: Version $(VERSION) is already set in version.txt"; exit 1; \
	fi; \
	tag="v$(VERSION)"; \
	branch=$$(git branch --show-current); \
	test -n "$$branch" || { echo "ERROR: Detached HEAD"; exit 1; }; \
	git diff --quiet && git diff --cached --quiet && \
	test -z "$$(git ls-files --others --exclude-standard)" || \
		{ echo "ERROR: Working tree is not clean"; exit 1; }; \
	git remote get-url $(REMOTE) >/dev/null 2>&1 || \
		{ echo "ERROR: Remote not configured"; exit 1; }; \
	git rev-parse -q --verify "refs/tags/$$tag" >/dev/null && \
		{ echo "ERROR: Local tag $$tag already exists"; exit 1; } || true; \
	if git ls-remote --exit-code --tags $(REMOTE) "refs/tags/$$tag" >/dev/null 2>&1; then \
		echo "ERROR: Remote tag $$tag already exists"; exit 1; \
	fi; \
	echo "Preparing release $$tag"; \
	printf '%s\n' "$(VERSION)" > "$(VERSION_FILE)"; \
	git add "$(VERSION_FILE)"; \
	git commit -m "chore(release): $(VERSION)"; \
	git push $(REMOTE) "$$branch"; \
	git tag -a "$$tag" -m "$(APP_NAME) $(VERSION)"; \
	git push $(REMOTE) "$$tag"; \
	echo "Release $$tag pushed successfully"

