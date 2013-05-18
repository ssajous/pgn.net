﻿namespace ilf.pgn.Test

open ilf.pgn
open ilf.pgn.PgnParsers.Tag
open ilf.pgn.Test.TestBase

open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TagParserTests() = 
    [<TestMethod; ExpectedException(typeof<PgnFormatException>)>]
    member this.pTag_should_fail_if_expressions_starts_with_non_bracket() =
        tryParse pTag "test"

    [<TestMethod>]
    member this.pTag_should_accept_random_spaces_before_and_after_brackets() =
        tryParse pTag "          [Event \"Foo\"]"
        tryParse pTag "  \t \n  [Event \"Foo\"]"
        tryParse pTag "  \t \n  [Event \"Foo\"]"
        tryParse pTag "[  \t \tEvent \"Foo\"\t \n ]  \t \n   \t \n"
        tryParse pTag "  \t \n  [  \t \n   \t \n Event \"Foo\"  \t \n   \t \n ]  \t\n\t \n   \t\t\n\n   \t \n\n "

    [<TestMethod>]
    member this.pTag_should_accept_random_spaces_between_tag_name_and_value() =
        tryParse pTag "[Event\t \n   \t \"Foo\"]"
        tryParse pTag "[Event \"Foo\"]"
        tryParse pTag "[Event \t \n   \t\"Foo\"]"
        tryParse pTag "[  \t \tEvent         \"Foo\"\t \n ]  \t \n   \t \n"
        tryParse pTag "  \t \n  [  \t \n   \t \n Event \t \n   \t \n  \"Foo\"  \t \n   \t \n ]  \t\n\t \n   \t\t\n\n   \t \n\n "

    [<TestMethod>]
    ///<see href="see http://www.saremba.de/chessgml/standards/pgn/pgn-complete.htm#c8.1.1" />
    member this.pTag_should_allow_tag_names_from_SevenTagRoaster() =
        tryParse pTag "[Date \"2013.05.18\"]"

        let basicTagNames = ["Event"; "Site"; "Round"; "White"; "Black"; "Result"]
        let parseTag x = tryParse pTag ("["+ x + " \"Foo\"]")
        List.map parseTag basicTagNames |> ignore
        ()

    [<TestMethod>]
    ///<see href="http://www.saremba.de/chessgml/standards/pgn/pgn-complete.htm#c9" />
    member this.pTag_should_allow_suplemental_tag_names() =
        let allowedTagNames = 
            ["WhiteTitle"; "BlackTitle"; "WhiteElo"; "BlackElo"; "WhiteUSCF"; "BlackUSCF"; "WhiteNA"; "BlackNA"; "WhiteType"; "BlackType"; 
            "EventDate"; "EventSponsor"; "Section"; "Stage"; "Board";
            "Opening"; "Variation"; "SubVariation";
            "ECO"; "NIC"; "Time"; "UTCTime"; "UTCDate";
            "TimeControl";
            "SetUp"; "FEN";
            "Termination";
            "Annotator"; "Mode"; "PlyCount"]
        let parseTag x = tryParse pTag ("["+ x + " \"Foo\"]")
        List.map parseTag allowedTagNames |> ignore
        ()

    [<TestMethod>]
    member this.pTag_should_create_a_Tag_object_from_a_valid_tag() =
        let tag= parse pTag "[Date \"2013.05.15\"]"
        Assert.IsInstanceOfType(tag, typeof<PgnTag>)
        Assert.AreEqual("Date", tag.Name)
        Assert.AreEqual(Some 2013, (tag :?> PgnDateTag).Year)
        Assert.AreEqual(Some 5, (tag :?> PgnDateTag).Month)
        Assert.AreEqual(Some 15, (tag :?> PgnDateTag).Day)

