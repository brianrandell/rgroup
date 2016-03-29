function addLikeHandler(e) {
    var d = e.data;
    var json = JSON.stringify({ "LikeKind": d.Kind, "Set": true });
    $.ajax
    ({
        type: "POST",
        url: d.Url,
        contentType: 'application/json',
        data: json,
        success: function () {
            d.OnComplete();
        }
    });
}
function removeLikeHandler(e) {
    var d = e.data;
    var json = JSON.stringify({ "LikeKind": d.Kind, "Set": false });
    $.ajax
    ({
        type: "POST",
        url: d.Url,
        contentType: 'application/json',
        data: json,
        success: function () {
            d.OnComplete();
        }
    });
}

function makeLikeable(userId, likeContainer, entry, likeCompleteCallback) {
    var lc = $(likeContainer);
    lc.empty();
    var showDiv = $("<div class='likes'/>");
    lc.append(showDiv);

    var thisUserLikeKinds = [];
    for (var lgi = 0; lgi < entry.LikeGroups.length; ++lgi) {
        var likeGroup = entry.LikeGroups[lgi];
        var indexOfThisUser = null;
        for (var ui = 0; ui < likeGroup.Users.length; ++ui) {
            if (likeGroup.Users[ui].Id == userId) {
                thisUserLikeKinds.push(likeGroup.Kind);
                indexOfThisUser = ui;
                break;
            }
        }
        if (indexOfThisUser != null) {
            likeGroup.Users.splice(indexOfThisUser, 1);
        }

        if (likeGroup.Users.length == 0) {
            continue;
        }

        var message = "";
        if (likeGroup.Users.length < 4) {
            for (var ui = 0; ui < likeGroup.Users.length; ++ui) {
                if (ui > 0) {
                    message += ui == (likeGroup.Users.length - 1)
                        ? ", and " : ", ";
                }
                message += likeGroup.Users[ui].Name;
            }
        } else {
            message = likeGroup.User.length.toString() + " people ";
        }

        switch (likeGroup.Kind) {
            case "Like":
                message += (likeGroup.Users.length > 1) ? " like" : " likes";
                break;
            case "Hug":
                message += " hugged";
                break;
            case "Frown":
                message += " frowned at";
                break;
            default:
                continue;
        }

        message += " this";

        showDiv.append($("<span>" + message + "</span>"));
    }

    if (entry.UserId != userId) {
        var addDiv = $("<div class='add'/>");
        lc.append(addDiv);

        function addLikeLinks(kind, unkind) {
            if (thisUserLikeKinds.indexOf(kind) < 0) {
                var addKind = $("<a href='#'>" + kind + "</a>");
                addDiv.append(addKind);
                addKind.click({ Url: entry.LikeUrl, Kind: kind, OnComplete: likeCompleteCallback }, addLikeHandler);
            } else {
                var removeKind = $("<a href='#'>" + unkind + "</a>");
                addDiv.append(removeKind);
                removeKind.click({ Url: entry.LikeUrl, Kind: kind, OnComplete: likeCompleteCallback }, removeLikeHandler);
            }
        }

        addLikeLinks("Like", "Unlike");
        addLikeLinks("Frown", "Unfrown");
        addLikeLinks("Hug", "Unhug");
    }
}