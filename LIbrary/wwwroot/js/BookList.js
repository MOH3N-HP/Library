$(document).ready(function () {
    $('#searchForm').on('submit', function (e) {
        e.preventDefault();

        var userInput = $('#searchInput').val();
        var url = searchUrl;

        $('#searchMessage').empty();
        $('#booksTableBody').html('<tr><td colspan="4">Loading books...</td></tr>');

        $.ajax({
            url: url,
            type: 'GET',
            data: { userInput: userInput },
            dataType: 'json',
            success: function (bookDtos) {
                console.log("Book DTOs received:", bookDtos);

                var tableBodyHtml = '';
                if (bookDtos && bookDtos.length > 0) {
                    $.each(bookDtos, function (index, book) {
                        tableBodyHtml += '<tr>';
                        tableBodyHtml += '<td>' + (book.title || 'N/A') + '</td>';
                        tableBodyHtml += '<td>' + (book.description || 'N/A') + '</td>';
                        tableBodyHtml += '<td>' + (book.libraryName || 'N/A') + '</td>';
                        tableBodyHtml += '<td>';
                        tableBodyHtml += '<div class="dropdown-center">';
                        tableBodyHtml += '<button class="btn dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                        tableBodyHtml += '<i class="fa fa-search-plus"></i></button>';
                        tableBodyHtml += '<ul class="flex-grow-1 dropdown-menu">';
                        tableBodyHtml += '<li class="dropdown-item" onclick="event.stopPropagation();"><b>Id:</b></li>';
                        tableBodyHtml += '<li class="dropdown-item" onclick="event.stopPropagation();">' + (book.id || 'N/A') + '</li>';
                        tableBodyHtml += '<li class="dropdown-item" onclick="event.stopPropagation();"><b>Authors:</b></li>';
                        if (book.authors && book.authors.length > 0) {
                            $.each(book.authors, function (idx, authorName) {
                                tableBodyHtml += '<li class="dropdown-item" onclick="event.stopPropagation();" style="text-align:start">' + authorName + '</li>';
                            });
                        } else {
                            tableBodyHtml += '<li class="dropdown-item" onclick="event.stopPropagation();" style="text-align:start">No Authors</li>';
                        }
                        tableBodyHtml += '</ul></div>';
                        tableBodyHtml += '<div class="dropdown-center">';
                        tableBodyHtml += '<button class="btn dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">';
                        tableBodyHtml += '<i class="fa fa-ellipsis-h"></i></button>';
                        tableBodyHtml += '<div class="dropdown-menu" aria-labelledby="dropdownMenuButton">';
                        tableBodyHtml += '<a class="dropdown-item ajax-delete-book" href="#" data-book-id="' + (book.id || '') + '">Delete</a>';
                        tableBodyHtml += '<a class="dropdown-item" href="/Book/Update?bookId=' + (book.id || '') + '">Update</a>';
                        tableBodyHtml += '</div></div>';
                        tableBodyHtml += '</td>';
                        tableBodyHtml += '</tr>';
                    });
                } else {
                    tableBodyHtml = '<tr><td colspan="4">No books found matching your criteria.</td></tr>';
                }
                $('#booksTableBody').html(tableBodyHtml);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error("AJAX Error:", textStatus, errorThrown, jqXHR.responseText);
                $('#searchMessage').text('An error occurred during search. Please try again.').css('color', 'red');
                $('#booksTableBody').html('<tr><td colspan="4">Failed to load books.</td></tr>');
            }
        });
    });


    $('.books-list').on('click', '.ajax-delete-book', function (e) {
        e.preventDefault();

        var $link = $(this);
        var bookIdToDelete = $link.data('book-id');

        if (!bookIdToDelete) {
            alert("Book ID not found for deletion.");
            return;
        }

        if (confirm("Are you sure you want to delete book ID " + bookIdToDelete + "? This cannot be undone.")) {
            $.ajax({
                url: `/Book/Delete/${bookIdToDelete}`,
                type: 'DELETE',
                success: function (response) {
                    alert('Book ID ' + bookIdToDelete + ' deleted successfully!');
                    console.log("nailed");
                    $link.closest('tr').remove();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var errorMessage = "Failed to delete book. ";
                    if (jqXHR.status === 404) {
                        errorMessage += "Book not found.";
                    } else if (jqXHR.status === 400) {
                        errorMessage += "Invalid request: " + jqXHR.responseText;
                    } else {
                        errorMessage += jqXHR.responseText || errorThrown;
                    }
                    alert(errorMessage);
                    console.error("Error deleting book:", textStatus, errorThrown, jqXHR.responseText);
                }
            });
        }
    });
});