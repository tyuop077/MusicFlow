class fm {
    static delete(id) {
        $.ajax({
            type: "POST",
            url: "/forum/1/deleteMessage",
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                mid: id
            },
            complete: (res) => {
                if (res.status === 200) {
                    alert("Deleted successfuly");
                    location.reload();
                } else if (res.status === 404) {
                    alert("Message was not found")
                } else alert("Unknown error");
            }
        })
    }
    static edit(id) {
        $.ajax({
            type: "POST",
            url: "/forum/1/editMessage",
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                mid: id,
                content: prompt("New message")
            },
            complete: (res) => {
                if (res.status === 200) {
                    alert("Message was edited successfuly");
                    location.reload();
                } else if (res.status === 404) {
                    alert("Message was not found")
                } else alert("Unknown error");
            }
        })
    }
    static reply(id) {
        $('input[name="rid"]').val(id);
        $('label[for="content"]').text("Reply to message");
        $('#unreplyBtn').show();
        $("html, body").animate({ scrollTop: $(document).height() }, "slow");
    }
    static unreply() {
        $('input[name="rid"]').val(null);
        $('#unreplyBtn').hide();
        $('label[for="content"]').text("Send a message");
    }
}

$('#unreplyBtn').hide();
