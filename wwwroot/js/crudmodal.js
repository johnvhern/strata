window.AppUI = (function () {
    function showToast(message, type = "success") {
        let background = "#198754";

        if (type === "error") background = "#dc3545";
        if (type === "warning") background = "#ffc107";
        if (type === "info") background = "#0d6efd";

        Toastify({
            text: message,
            duration: 2200,
            close: true,
            gravity: "top",
            position: "right",
            stopOnFocus: true,
            style: {
                background: background
            }
        }).showToast();
    }

    function resetModalState() {
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
        $('body').css({
            overflow: '',
            paddingRight: ''
        });
    }

    function parseDynamicForm(formSelector) {
        if (!formSelector) return;

        const form = $(formSelector);
        if (!form.length) return;

        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
    }

    function showModal(html, modalSelector, formSelector, placeholderSelector) {
        resetModalState();

        const placeholder = $(placeholderSelector);
        placeholder.html(html);

        const modalEl = document.querySelector(modalSelector);

        if (!modalEl) {
            console.error(`Modal element not found for selector: ${modalSelector}`);
            console.error('Injected HTML:', html);
            showToast('Modal could not be loaded.', 'error');
            return;
        }

        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);

        parseDynamicForm(formSelector);
        modal.show();

        $(modalEl).off('hidden.bs.modal').on('hidden.bs.modal', function () {
            resetModalState();
            placeholder.empty();
        });
    }

    function initAjaxModal(options) {
        const {
            triggerSelector,
            placeholderSelector,
            modalSelector,
            formSelector,
            saveButtonSelector,
            successRedirectUrl,
            successQueryKey,
            successMessage,
            errorMessage
        } = options;

        const placeholder = $(placeholderSelector);

        $('body').on('click', triggerSelector, function (e) {
            e.preventDefault();
            e.stopPropagation();
            
            const url = $(this).data('url');

            $.get(url).done(function (html) {
                showModal(html, modalSelector, formSelector, placeholderSelector);
            });
        });

        if (saveButtonSelector && formSelector) {
            placeholder.on('click', saveButtonSelector, function (e) {
                e.preventDefault();
                e.stopPropagation();

                const form = $(formSelector);
                if (!form.length) return;

                $.ajax({
                    type: 'POST',
                    url: form.attr('action'),
                    data: form.serialize(),
                    success: function (response) {
                        if (typeof response === 'string') {
                            showModal(response, modalSelector, formSelector, placeholderSelector);
                        } else if (response.success) {
                            const modalEl = document.querySelector(modalSelector);
                            const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
                            modal.hide();

                            if (successRedirectUrl && successQueryKey) {
                                window.location.href = successRedirectUrl + '?' + successQueryKey + '=1';
                            }
                        }
                    },
                    error: function () {
                        showToast(errorMessage || "Something went wrong.", "error");
                    }
                });
            });
        }
    }

    function showToastFromQuery(paramName, message, type = "success") {
        const params = new URLSearchParams(window.location.search);

        if (params.get(paramName) === '1') {
            showToast(message, type);

            const cleanUrl = window.location.pathname;
            window.history.replaceState({}, document.title, cleanUrl);
        }
    }

    return {
        showToast,
        initAjaxModal,
        showToastFromQuery
    };
})();