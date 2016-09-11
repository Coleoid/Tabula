Data migration

Required fields with optional source Data
    Collect answers from
        customers/workers in domain?
        other places in source data with approximate values?
            if another location has the exact values, we've solved our problems
    Release requirement?
        Feature dependent on requirement--flag off?
    Give an obviously wrong value to serve as a sentinel?
        Dependent feature uses data, so it needs to omit the records with sentinel values?
        In which case, are we better off than if we'd released the requirement?

Fields with different source value types
    10 digit number field storing phone numbers, with unconstrained text as source
    Text with longer text as source
    destination holds an enum
        Translation table

Simply missing data
    Feature dependencies?

Different arity
    Source M:1, destination 1:1
        Pick a winner via some method?
        Extend destination system to M:1?

Reused fields and tables

Elder
