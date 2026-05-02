using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_System
{
    // ── PrescriptionItem ──────────────────────────────────────────────────────
    public class PrescriptionItem
    {
        public Medicine Medicine      { get; set; }
        public int      Quantity      { get; set; }
        public int      TimesPerDay   { get; set; }
        public int      NumberOfDays  { get; set; }

        public PrescriptionItem() { }

        public PrescriptionItem(Medicine medicine, int qty, int times, int days)
        {
            Medicine     = medicine;
            Quantity     = qty;
            TimesPerDay  = times;
            NumberOfDays = days;
        }

        /// <summary>Total cost for this line item (price x quantity).</summary>
        public double TotalPrice() => Medicine.Price * Quantity;

        public override string ToString() =>
            $"{Medicine.Name} | Qty: {Quantity} | {TimesPerDay}x/day | {NumberOfDays} days | {TotalPrice():F2} EGP";
    }

    // ── Prescription ──────────────────────────────────────────────────────────
    /// <summary>
    /// Represents a full prescription issued by a doctor for a patient.
    /// Holds a list of PrescriptionItem entries and exposes helpers used
    /// by both the MainForm UI and any business-logic layer.
    /// </summary>
    public class Prescription
    {
        // ── Identity ──────────────────────────────────────────────────────────
        private static int _nextId = 1;
        public int      PrescriptionId { get; private set; }
        public string   DoctorName     { get; set; }
        public string   PatientName    { get; set; }
        public DateTime IssuedDate     { get; private set; }

        // ── Items ─────────────────────────────────────────────────────────────
        private readonly List<PrescriptionItem> _items = new List<PrescriptionItem>();

        /// <summary>Read-only view of the current prescription items.</summary>
        public IReadOnlyList<PrescriptionItem> Items => _items.AsReadOnly();

        // ── Constructors ──────────────────────────────────────────────────────
        public Prescription()
        {
            PrescriptionId = _nextId++;
            IssuedDate     = DateTime.Now;
        }

        public Prescription(string doctorName, string patientName) : this()
        {
            DoctorName  = doctorName;
            PatientName = patientName;
        }

        // ── Item management ───────────────────────────────────────────────────

        /// <summary>
        /// Adds a new item or increases the quantity of an existing entry
        /// for the same medicine.
        /// </summary>
        public void AddItem(Medicine medicine, int qty, int timesPerDay, int days)
        {
            if (medicine == null)   throw new ArgumentNullException(nameof(medicine));
            if (qty        <= 0)    throw new ArgumentOutOfRangeException(nameof(qty),         "Quantity must be > 0.");
            if (timesPerDay <= 0)   throw new ArgumentOutOfRangeException(nameof(timesPerDay),  "Times per day must be > 0.");
            if (days        <= 0)   throw new ArgumentOutOfRangeException(nameof(days),         "Number of days must be > 0.");

            int idx = _items.FindIndex(x =>
                x.Medicine.Name.Equals(medicine.Name, StringComparison.OrdinalIgnoreCase));

            if (idx >= 0)
            {
                var old = _items[idx];
                _items[idx] = new PrescriptionItem(old.Medicine, old.Quantity + qty,
                                                   old.TimesPerDay, old.NumberOfDays);
            }
            else
            {
                _items.Add(new PrescriptionItem(medicine, qty, timesPerDay, days));
            }
        }

        /// <summary>Removes the item whose medicine name matches (case-insensitive).</summary>
        /// <returns>true if an item was removed; otherwise false.</returns>
        public bool RemoveItem(string medicineName)
        {
            int idx = _items.FindIndex(x =>
                x.Medicine.Name.Equals(medicineName, StringComparison.OrdinalIgnoreCase));
            if (idx < 0) return false;
            _items.RemoveAt(idx);
            return true;
        }

        /// <summary>Removes all items from the prescription.</summary>
        public void Clear() => _items.Clear();

        // ── Totals ────────────────────────────────────────────────────────────

        /// <summary>Returns the total cost of all items.</summary>
        public double GetTotal()
        {
            double total = 0;
            foreach (var item in _items) total += item.TotalPrice();
            return total;
        }

        /// <summary>Returns true when there are no items.</summary>
        public bool IsEmpty => _items.Count == 0;

        // ── Summary ───────────────────────────────────────────────────────────

        /// <summary>
        /// Builds a plain-text summary suitable for showing in a MessageBox
        /// or printing on a receipt.
        /// </summary>
        public string GetSummary()
        {
            if (IsEmpty) return "Prescription is empty.";

            var sb = new StringBuilder();
            sb.AppendLine("======================================");
            sb.AppendLine($"  PRESCRIPTION  #{PrescriptionId}");
            sb.AppendLine("======================================");
            if (!string.IsNullOrWhiteSpace(DoctorName))
                sb.AppendLine($"  Doctor  : Dr. {DoctorName}");
            if (!string.IsNullOrWhiteSpace(PatientName))
                sb.AppendLine($"  Patient : {PatientName}");
            sb.AppendLine($"  Date    : {IssuedDate:dd/MM/yyyy  HH:mm}");
            sb.AppendLine("--------------------------------------");
            foreach (var item in _items)
                sb.AppendLine($"  * {item}");
            sb.AppendLine("--------------------------------------");
            sb.AppendLine($"  TOTAL   : {GetTotal():F2} EGP");
            sb.AppendLine("======================================");
            return sb.ToString();
        }

        public override string ToString() =>
            $"Prescription #{PrescriptionId} | {_items.Count} item(s) | Total: {GetTotal():F2} EGP";
    }
}
