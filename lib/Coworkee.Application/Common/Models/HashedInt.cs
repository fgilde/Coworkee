using System;
using System.Linq;
using HashidsNet;
using Nextended.Core.Helper;

namespace lib.Coworkee.Application.Common.Models;

public class HashedInt
{
    private static string _salt = "Coworkee-Salt-E51FCEAC-B99F-462E-8D35-8F087033447C";
    private static int _hashMinLength = 10;
    private static bool _allowWithExplicitId;

    internal static void SetSettings(int minLength, string salt, bool allowExplicitId)
    {
        _hashMinLength = minLength;
        _salt = salt;
        _allowWithExplicitId = allowExplicitId;
    }

    private readonly Hashids _hasher;
    private readonly int[] _values;
    private readonly string _hash;

    public HashedInt(string hash) : this()
    {
        if (_allowWithExplicitId && int.TryParse(hash, out var id))
            _values = new[] {id};
        else
            _hash = hash;
    }

    public HashedInt(params int[] id) : this()
    {
        _values = id;
    }

    private HashedInt()
    {
        _hasher = new Hashids(_salt, _hashMinLength);
    }

    public int[] Ids => string.IsNullOrEmpty(_hash) ? _values : _hasher.Decode(_hash);
    public int Id => Ids.FirstOrDefault();
    public string Hash => !string.IsNullOrEmpty(_hash) ? _hash : _hasher.Encode(_values);

    public override string ToString()
    {
        return Hash;
    }

    public static implicit operator string(HashedInt t) => t.Hash;
    public static implicit operator int[](HashedInt t) => t.Ids;
    public static implicit operator int(HashedInt t) => t.Id;
    public static implicit operator HashedInt(string s) => new(s);
    public static implicit operator HashedInt(int i) => new(i);
    public static implicit operator HashedInt(int[] i) => new(i);
}