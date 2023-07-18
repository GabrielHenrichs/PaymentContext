using System.Diagnostics.Contracts;
using Flunt.Notifications;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Repositories;
using PaymentContext.Domain.Services;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Domian.Enums;
using PaymentContext.Shared.Commands;
using PaymentContext.Shared.Handlers;

namespace PaymentContext.Domain.Handlers;

public class SubscriptionHandler :
    Notifiable<Notification>,
    IHandler<CreateBoletoSubscriptionCommand>,
    IHandler<CreatePayPalSubscriptionCommand>,
    IHandler<CreateCreditCardSubscriptionCommand>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEmailService _emailService;

    public SubscriptionHandler(IStudentRepository studentRepository, IEmailService emailService)
    {
        _studentRepository = studentRepository;
        _emailService = emailService;
    }

    public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
    {
        // Fail Fast Validations
        command.Validate();
        if (!command.IsValid)
        {
            AddNotifications(command);
            return new CommandResult(false, "Não foi possível realizar sua assinatura");
        }

        // Verificar se Documento já está cadastrado
        if (_studentRepository.DocumentExists(command.Document))
            AddNotification("Document", "Este CPF já está em uso");

        // Verificar se Email já está cadastrado
        if (_studentRepository.EmailExists(command.Email))
            AddNotification("Email", "Este E-mail já está em uso");

        // Gerar os VOs
        var name = new Name(command.FirstName, command.LastName);
        var document = new Document(command.Document, EDocumentType.CPF);
        var email = new Email(command.Email);
        var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

        // Gerar as Entidades
        var student = new Student(name, document, email);
        var subscription = new Subscription(DateTime.Now.AddMonths(1));
        var payment = new BoletoPayment(command.BarCode, command.BoletoNumber, command.PaidDate, command.ExpireDate, command.Total,
            command.TotalPaid, command.Payer, new Document(command.PayerDocument, command.PayerDocumentType), address, email);

        // Relacionamentos
        subscription.AddPayment(payment);
        student.AddSubscription(subscription);

        // Aplicar as Validações
        AddNotifications(name, document, email, address, student, subscription, payment);

        // Checar as notificações
        if (!IsValid)
            return new CommandResult(false, "Não foi possível realizar sua assinatura");

        // Salvar as Informações
        _studentRepository.CreateSubscription(student);

        // Enviar Email de boas vindas
        _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo ao balta.io", "Sua assinatura foi criada");

        // Retornar informações
        return new CommandResult(true, "Assinatura realizada com sucesso");
    }

    public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
    {
        // Fail Fast Validations
        command.Validate();
        if (!command.IsValid)
        {
            AddNotifications(command);
            return new CommandResult(false, "Não foi possível realizar sua assinatura");
        }

        // Verificar se Documento já está cadastrado
        if (_studentRepository.DocumentExists(command.Document))
            AddNotification("Document", "Este CPF já está em uso");

        // Verificar se Email já está cadastrado
        if (_studentRepository.EmailExists(command.Email))
            AddNotification("Email", "Este E-mail já está em uso");

        // Gerar os VOs
        var name = new Name(command.FirstName, command.LastName);
        var document = new Document(command.Document, EDocumentType.CPF);
        var email = new Email(command.Email);
        var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

        // Gerar as Entidades
        var student = new Student(name, document, email);
        var subscription = new Subscription(DateTime.Now.AddMonths(1));
        var payment = new PayPalPayment(command.TransactionCode, command.PaidDate, command.ExpireDate, command.Total,
            command.TotalPaid, command.Payer, new Document(command.PayerDocument, command.PayerDocumentType), address, email);

        // Relacionamentos
        subscription.AddPayment(payment);
        student.AddSubscription(subscription);

        // Aplicar as Validações
        AddNotifications(name, document, email, address, student, subscription, payment);

        // Checar as notificações
        if (!IsValid)
            return new CommandResult(false, "Não foi possível realizar sua assinatura");

        // Salvar as Informações
        _studentRepository.CreateSubscription(student);

        // Enviar Email de boas vindas
        _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo ao balta.io", "Sua assinatura foi criada");

        // Retornar informações
        return new CommandResult(true, "Assinatura realizada com sucesso");
    }

    public ICommandResult Handle(CreateCreditCardSubscriptionCommand command)
    {
        // Fail Fast Validations
        command.Validate();
        if (!command.IsValid)
        {
            AddNotifications(command);
            return new CommandResult(false, "Não foi possível realizar sua assinatura");
        }

        // Verificar se Documento já está cadastrado
        if (_studentRepository.DocumentExists(command.Document))
            AddNotification("Document", "Este CPF já está em uso");

        // Verificar se Email já está cadastrado
        if (_studentRepository.EmailExists(command.Email))
            AddNotification("Email", "Este E-mail já está em uso");

        // Gerar os VOs
        var name = new Name(command.FirstName, command.LastName);
        var document = new Document(command.Document, EDocumentType.CPF);
        var email = new Email(command.Email);
        var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

        // Gerar as Entidades
        var student = new Student(name, document, email);
        var subscription = new Subscription(DateTime.Now.AddMonths(1));
        var payment = new CreditCardPayment(command.CardHolderName, command.CardNumber, command.LastTransactionNumber, command.PaidDate, command.ExpireDate, command.Total,
            command.TotalPaid, command.Payer, new Document(command.PayerDocument, command.PayerDocumentType), address, email);

        // Relacionamentos
        subscription.AddPayment(payment);
        student.AddSubscription(subscription);

        // Aplicar as Validações
        AddNotifications(name, document, email, address, student, subscription, payment);

        // Checar as notificações
        if (!IsValid)
            return new CommandResult(false, "Não foi possível realizar sua assinatura");

        // Salvar as Informações
        _studentRepository.CreateSubscription(student);

        // Enviar Email de boas vindas
        _emailService.Send(student.Name.ToString(), student.Email.Address, "Bem vindo ao balta.io", "Sua assinatura foi criada");

        // Retornar informações
        return new CommandResult(true, "Assinatura realizada com sucesso");
    }
}
