$("#auth").submit(e => {
    e.preventDefault();
    $.ajax({
        type: "POST",
        dataType: "json",
        data: $("#auth").serialize(),
        success: data => {
            if (data.success) window.location.href = "/";
            $("#tooltip").text(data.error)
        }
    });
});