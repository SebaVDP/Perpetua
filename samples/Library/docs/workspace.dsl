workspace "Library" "Living architecture demo - the containers and relationships are generated from the code (Perpetua attributes); only the context is written by hand." {

    !identifiers hierarchical

    model {
        member = person "Member" "Reserves and borrows books from the library."

        Library = softwareSystem "Library" "The online library." {
            !include Library/containers
        }

        !include Library/relationships

        member -> Library.Website "Reserves and borrows via" "HTTPS"
    }

    views {
        systemContext Library "Context" {
            include *
            autolayout lr
        }

        container Library "Containers" {
            include *
            autolayout lr
        }

        styles {
            element "Person" {
                shape person
                background #08427b
                color #ffffff
            }
            element "Container" {
                background #1168bd
                color #ffffff
            }
            element "Event" {
                shape pipe
                background #7b4fb0
                color #ffffff
            }
        }
    }
}
