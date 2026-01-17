using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.VectorData;

namespace Documind.Domain;

public class DocumentRecord
{
    [VectorStoreKey]
    public Guid Id { get; set; }

    [VectorStoreData]
    public string Content { get; set; } = string.Empty;

    // Gemini embedding-004 output is 768 dimensions
    [VectorStoreVector(768)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}