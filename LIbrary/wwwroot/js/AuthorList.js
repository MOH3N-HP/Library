$(document).ready(function () {
    $('#searchForm').on('submit', function (e) {
        e.preventDefault();

        var userInput = $('#searchInput').val();
        var url = searchUrl;

        $('#searchMessage').empty();
        $('#authorsTableBody').html('<tr><td colspan="4">Loading authors...</td></tr>');

        $.ajax({
            url: url,
            type: 'GET',
            data: { userInput: userInput },
            dataType: 'json',
            success: function (authorDtos) {
                console.log("Author DTOs received:", authorDtos);

                var tableBodyHtml = '';
                if (authorDtos && authorDtos.length > 0) {
                    $.each(authorDtos, function (index, author) {
                        tableBodyHtml += '<tr>';
                        tableBodyHtml += '<td>' + (author.name || 'N/A') + '</td>';
                        tableBodyHtml += '<td>' + (author.age || 'N/A') + '</td>';
                        tableBodyHtml += '<td>';
                        tableBodyHtml += '<div class="dropdown-center">';
                        tableBodyHtml += '<button class="btn dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                        tableBodyHtml += '<i class="fa fa-search-plus"></i></button>';
                        tableBodyHtml += '<ul class="flex-grow-1 dropdown-menu">';
                        tableBodyHtml += '<li class="dropdown-item" onclick="event.stopPropagation();"><b>Id:</b></li>';
                        tableBodyHtml += '<li class="dropdown-item" onclick="event.stopPropagation();">' + (author.id || 'N/A') + '</li>';
                        tableBodyHtml += '<li class="dropdown-item" onclick="event.stopPropagation();"><b>Books:</b></li>';
                        if (author.books && author.books.length > 0) {
                            $.each(author.books, function (idx, bookTitle) {
                                tableBodyHtml += '<li class="dropdown-item" onclick="event.stopPropagation();" style="text-align:start">' + bookTitle + '</li>';
                            });
                        } else {
                            tableBodyHtml += '<li class="dropdown-item" onclick="event.stopPropagation();" style="text-align:start">No Books</li>';
                        }
                        tableBodyHtml += '</ul></div>';
                        tableBodyHtml += '<div class="dropdown-center">';
                        tableBodyHtml += '<button class="btn dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                        tableBodyHtml += '<i class="fa fa-ellipsis-h"></i></button>';
                        tableBodyHtml += '<div class="dropdown-menu" aria-labelledby="dropdownMenuButton">';
                        tableBodyHtml += '<a class="dropdown-item ajax-delete-author" href="#" data-author-id="' + (author.id || '') + '">Delete</a>';
                        tableBodyHtml += '<a class="dropdown-item" href="/Author/Update?authorId=' + (author.id || '') + '">Update</a>';
                        tableBodyHtml += '</div></div>';
                        tableBodyHtml += '</td>';
                        tableBodyHtml += '</tr>';
                    });
                } else {
                    tableBodyHtml = '<tr><td colspan="4">No authors found matching your criteria.</td></tr>';
                }
                $('#authorsTableBody').html(tableBodyHtml);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error("AJAX Error:", textStatus, errorThrown, jqXHR.responseText);
                $('#searchMessage').text('An error occurred during search. Please try again.').css('color', 'red');
                $('#authorsTableBody').html('<tr><td colspan="4">Failed to load authors.</td></tr>');
            }
        });

    });


    $('.books-list').on('click', '.ajax-delete-author', function (e) {
        e.preventDefault();

        var $link = $(this);
        var authorIdToDelete = $link.data('author-id');

        if (!authorIdToDelete) {
            alert("Author ID not found for deletion.");
            return;
        }

        if (confirm("Are you sure you want to delete author ID " + authorIdToDelete + "? This cannot be undone.")) {
            $.ajax({
                url: `/Author/Delete/${authorIdToDelete}`,
                type: 'DELETE',
                success: function (response) {
                    alert('Author ID ' + authorIdToDelete + ' deleted successfully!');
                    console.log("nailed");
                    $link.closest('tr').remove();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var errorMessage = "Failed to delete author. ";
                    if (jqXHR.status === 404) {
                        errorMessage += "Author not found.";
                    } else if (jqXHR.status === 400) {
                        errorMessage += "Invalid request: " + jqXHR.responseText;
                    } else {
                        errorMessage += jqXHR.responseText || errorThrown;
                    }
                    alert(errorMessage);
                    console.error("Error deleting author:", textStatus, errorThrown, jqXHR.responseText);
                }
            });
        }
    });
});