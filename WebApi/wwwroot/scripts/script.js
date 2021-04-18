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
    this.Sum.subscribe(function (item) {
        self.Sum(parseInt(item));
    });
}



function Invoice(item) {
    var self = this;
    self.Id = item.id;
    self.InvoiceNumber = item.invoiceNumber;
    self.ExecutionDate = new Date(item.executionDate).toLocaleString();
    self.Sum = item.sum;
    self.EmployeeName = item.employeeName;
    self.Details = [];
    item.details.forEach(function (detail) {
        self.Details.push(new DetailsItem(detail));
    });
}

function Employee(item) {
    this.Id = item.id;
    this.Name = item.name;
    this.Surname = item.surname;
    this.DateOfBirth = new Date(item.dateOfBirth).toLocaleDateString('en-GB');
}

function MainVM() {
    self.Invoices = ko.observableArray([]);
    self.UpdateInvoiceTable = function () {
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
    self.UpdateInvoiceTable();
    self.Details = ko.observableArray([]);
    self.ShowDetails = function (invoice) {
        self.Details(invoice.Details);
        $('#detailsModal').modal();
    }

    self.Employees = ko.observableArray([]);
    self.UpdateEmployeeTable = function () {
        $.getJSON("/api/employee",
            function (data) {
                var mappedData = $.map(data,
                    function (item) {
                        return new Employee(item);
                    });
                self.Employees(mappedData);
            }
        );
    }
    self.UpdateEmployeeTable();
}

$(document).ready(function () {
    ko.applyBindings(new MainVM());
});
