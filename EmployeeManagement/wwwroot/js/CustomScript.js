function confirmDelete(unique_id, is_delete_clicked)
{
    var delete_span = 'deleteSpan_' + unique_id;
    var confirm_delete_span = 'confirmDeleteSpan_' + unique_id;

    if (is_delete_clicked)
    {
        $('#' + delete_span).hide();
        $('#' + confirm_delete_span).show();
    }
    else
    {
        $('#' + delete_span).show();
        $('#' + confirm_delete_span).hide();
    }
}