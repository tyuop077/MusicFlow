let redirect;

$("#auth").submit(e => {
    e.preventDefault();
    $.ajax({
        type: "POST",
        dataType: "json",
        data: $("#auth").serialize(),
        success: data => {
            if (data.success) window.location.href = redirect ?? "/";
            else $("#tooltip").text(data.error)
        }
    });
});

const params = new URLSearchParams(window.location.search);
for (const [key, value] of params) {
    if (key === "r") {
        redirect = value;
        $("#tooltip").text("You have to be authorized to view this page");
    }
}