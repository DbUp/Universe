locals {
  repositories = toset(split("\n", file("../AllProviders.txt") ))
}


terraform {
  required_providers {
    github = {
      source  = "integrations/github"
      version = ">=5.40.0"
    }
  }
  
  cloud {
    organization = "DbUp"

    workspaces {
      name = "GithubBranchProtections"
    }
  }
}

provider "github" {
  owner = "DbUp"
}

resource "github_branch_protection" "main_branch" {
  for_each            = local.repositories
  repository_id       = each.value
  pattern             = "main"
  enforce_admins      = false
  allows_deletions    = false
  allows_force_pushes = false

  restrict_pushes {
    blocks_creations = true
  }

  required_pull_request_reviews {
    required_approving_review_count = 1
    dismiss_stale_reviews           = true
    require_last_push_approval      = true
  }

  required_status_checks {
    strict   = false
    contexts = ["build (pull_request)"]
  }
}

resource "github_branch_protection" "release_branches" {
  for_each            = local.repositories
  repository_id       = each.value
  pattern             = "release/**"
  enforce_admins      = false
  allows_deletions    = false
  allows_force_pushes = false

  restrict_pushes {
    blocks_creations = true
  }

  required_pull_request_reviews {
    required_approving_review_count = 1
    dismiss_stale_reviews           = true
    require_last_push_approval      = true
  }

  required_status_checks {
    strict   = false
    contexts = ["build (pull_request)"]
  }
}
