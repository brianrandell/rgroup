function emojify(c) {
    if (c) {
        c = emojione.toImage(c);

        var mentionregexp = /\B@([A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6})@\B/ig;
        var matches = []
        while (match = mentionregexp.exec(c)) {
            matches.push(match);
        }
        var adjust = 0;
        for (var i = 0; i < matches.length; ++i) {
            var match = matches[i];
            var email = match[1];
            var entry = userCompletionList[email];
            if (entry) {
                var userSpan = "<span class='mention'><img class='avatar' + src='"
                    + entry.AvatarUrl + "' />"
                    + entry.Name + "</span>";
                c = c.slice(0, adjust + match.index) + userSpan + c.slice(adjust + match.index + match[0].length);
                adjust += userSpan.length - match[0].length;
            }
        }
    }

    return c;
}
function previewEmojiElem(input, output) {
    output.html(emojione.toImage(input.val()));
}
function previewEmoji(inputId, outputId) {
    var input = $("#" + inputId);
    var output = $("#" + outputId)
    previewEmojiElem($(input), $(output));
}
