function addCommentHandler(e) {
    var d = e.data;
    var json = JSON.stringify({ "Message": d.TextArea.val() });
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

function makeCommentTarget(userId, userAvatarUrl, commentContainer, entry, commentCompleteCallback) {
    var cc = $(commentContainer);
    cc.empty();
    var existingComments = cc.append("<div class='comments'></div>");

    for (var i = 0; i < entry.Comments.length; ++i) {
        var comment = entry.Comments[i];
        var commentDiv = $(tmpl("tmpl-comment", comment));
        existingComments.append(commentDiv);

        var likeContainer = $("<div class='likeContainer'/>");
        commentDiv.append(likeContainer);
        makeLikeable(userId, likeContainer, comment, commentCompleteCallback);
    }

    var addComments = $(tmpl("tmpl-addcomment", { avatar: userAvatarUrl }));
    existingComments.append(addComments);
    var addPreviewButton = addComments.find(".btn-preview");

    var addTextArea = addComments.find(".txtin-comment");

    addPreviewButton.click(function () {
        previewEmojiElem(addTextArea, addPreviewElement);
    });

    enableEditAutoCompletion(addTextArea);

    var addButton = addComments.find(".btn-post");
    addButton.click({ Url: entry.CommentUrl, TextArea: addTextArea, OnComplete: commentCompleteCallback }, addCommentHandler);
}
