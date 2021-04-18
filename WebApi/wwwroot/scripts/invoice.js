
function DetailsItem(item) {
    var self = this;
    this.Id = ko.observable(0);
    this.Description = ko.observable('');
    this.Sum = ko.observable(0);
    if (typeof item !== 'undefined') {
        this.Id(item.id);
        this.Description(item.description);
        this.Sum(item.sum);
    }
    this.Sum.subscribe(function(item) {
        self.Sum(parseInt(item));
    });
}

function Employee(id, name) {
    this.Id = +id;
    this.Name = name;
}

function Invoice(item) {
    var self = this;
    self.Id = item.id;
    self.InvoiceNumber = item.invoiceNumber;
    self.ExecutionDate = new Date(item.executionDate).toLocaleString();
    self.Sum = item.sum;
    self.Employee = new Employee(item.employeeId, item.employeeName);
    self.Details = [];
    item.details.forEach(function (detail) {
        self.Details.push(new DetailsItem(detail));
    });
}

function ToDateTime(dateStr) {
    var dateParts = dateStr.slice(0, 10).split(".");
    var timeParts = dateStr.slice(12).split(":");
    var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0], +timeParts[0] + 3, +timeParts[1], +timeParts[2]);
    var isoDateStr = dateObject.toISOString();
    return isoDateStr.substr(0, isoDateStr.length - 1);
}

function InvoiceVM() {
    self.Invoices = ko.observableArray([]);
    self.UpdateTable = function () {
        $.getJSON("/api/invoice",
            function (data) {
                var mappedData = $.map(data,
                    function (item) {
                        return new Invoice(item);
                    });
                self.Invoices(mappedData);
            }
        );
    }
    self.UpdateTable();

    self.Details = ko.observableArray([]);

    self.ShowDetails = function (invoice) {
        self.Details(invoice.Details);
        $('#detailsModal').modal();
    }



    function ObservableInvoice(invoice) {
        var currentDate = new Date().toLocaleString();
        this.Id = ko.observable(0);
        this.InvoiceNumber = ko.observable('');
        this.ExecutionDate = ko.observable(ToDateTime(currentDate));
        this.Employee = ko.observable(new Employee(-1, ''));
        this.Details = ko.observableArray([]);
        this.Sum = ko.computed(function () {
            var sum = 0;
            Details().forEach(function(detail) {
                sum += detail.Sum();
            });
            return sum;
        });


        if (typeof invoice !== 'undefined') {
            this.Id(invoice.Id);
            this.InvoiceNumber(invoice.InvoiceNumber);
            this.Employee(invoice.Employee);
            this.Details(invoice.Details);


            this.ExecutionDate(ToDateTime(invoice.ExecutionDate));
        }
    }

    self.CurrentInvoice = ko.observable(new ObservableInvoice());
    self.CurrentInvoice.subscribe(function(invoice) {
        self.SelectedEmployee(invoice.Employee());
    });

    self.AvailableEmployees = ko.observableArray([]);
    self.GetEmployee = function () {
        $.getJSON("/api/employee",
            function (data) {
                var mappedData = $.map(data,
                    function (item) {
                        return new Employee(item.id, item.name + ' ' + item.surname);
                    });
                self.AvailableEmployees(mappedData);
            }
        );
    }
    self.GetEmployee();

    self.TryUpdate = function (invoice) {
        self.CurrentInvoice(new ObservableInvoice(invoice));
        self.Details(self.CurrentInvoice().Details().slice(0, self.CurrentInvoice().Details().length));
        $('#editInvoiceModalForm').modal();
    }

    self.Update = function() {
        self.CurrentInvoice().Details(self.Details());


        $.ajax({
            type: "PUT",
            url: "/api/Invoice/" + self.CurrentInvoice().Id(),
            data: ko.toJSON(self.CurrentInvoice()),
            contentType: "application/json",
            success: function () {
                self.UpdateTable();
            },
            error: function () {
                alert("Failed");
            }
        });
        $('#editInvoiceModalForm').modal('hide');

    }

    self.SelectedEmployee = ko.observable(self.AvailableEmployees()[2]);
    self.SelectedEmployee.subscribe(function (employee) {
        self.CurrentInvoice().Employee(employee);
    });



    self.AddDetails = function () {
        self.Details.push(new DetailsItem());
    }

    self.DeleteDetails = function (details) {
        self.Details.remove(function(det) {
            return det.Description() == details.Description() && det.Sum() == details.Sum();
        });
    }

    self.TryAddInvoice = function () {
        self.CurrentInvoice(new ObservableInvoice());
        self.Details(self.CurrentInvoice().Details().slice(0, self.CurrentInvoice().Details().length));
        $('#createInvoiceModalForm').modal();
    }

    self.AddInvoice = function () {
        self.CurrentInvoice().Details(self.Details());
        $.ajax({
            type: "POST",
            url: "/api/Invoice",
            data: ko.toJSON(self.CurrentInvoice()),
            contentType: "application/json",
            success: function () {
                self.UpdateTable();
            },
            error: function () {
                alert("Failed");
            }
        });
        $('#createInvoiceModalForm').modal('hide');
    }

    self.TryDeleteInvoice = function (invoice) {
        self.CurrentInvoice(new ObservableInvoice(invoice));
        $('#deleteInvoiceConfirmation').modal();
        console.log('here');
    }

    self.DeleteInvoice = function() {
        $.ajax({
            type: "DELETE",
            url: "/api/Invoice/" + self.CurrentInvoice().Id(),
            success: function () {
                self.UpdateTable();
            },
            error: function () {
                alert("Failed");
            }
        });
        $('#deleteInvoiceConfirmation').modal('hide');

    }
}

$(document).ready(function () {
    ko.applyBindings(new InvoiceVM());
});
