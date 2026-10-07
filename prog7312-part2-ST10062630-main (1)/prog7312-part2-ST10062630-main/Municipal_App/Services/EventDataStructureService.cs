using Municipal_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Municipal_App.Services
{
    public class EventDataStructureService : IEventDataStructureService
    {
        private readonly Dictionary<int, LocalEvent> _eventHashTable = new Dictionary<int, LocalEvent>();
        private readonly Dictionary<string, List<LocalEvent>> _eventsByCategory = new Dictionary<string, List<LocalEvent>>();
        private readonly SortedDictionary<DateTime, List<LocalEvent>> _eventsByDate = new SortedDictionary<DateTime, List<LocalEvent>>();
        private readonly HashSet<string> _uniqueCategories = new HashSet<string>();
        private readonly HashSet<DateTime> _uniqueDates = new HashSet<DateTime>();
        private readonly Stack<LocalEvent> _eventHistory = new Stack<LocalEvent>();
        private readonly Queue<LocalEvent> _eventQueue = new Queue<LocalEvent>();
        private readonly SortedDictionary<int, Queue<LocalEvent>> _priorityQueue = new SortedDictionary<int, Queue<LocalEvent>>();

        public void LoadEvents(List<LocalEvent> events)
        {
            InitializeWithEvents(events);
        }

        public void InitializeWithEvents(List<LocalEvent> events)
        {
            ClearAllStructures();

            foreach (var evt in events)
            {
                PushEventToHistory(evt);
                EnqueueEvent(evt);

                var daysUntilEvent = (evt.EventDate - DateTime.Now).Days;
                var priority = daysUntilEvent < 7 ? 1 : daysUntilEvent < 30 ? 2 : 3;
                EnqueuePriorityEvent(evt, priority);

                AddEventToHashTable(evt.Id, evt);
                AddEventToDictionary(evt.Category, evt);
                AddEventToSortedDictionary(evt.EventDate, evt);

                AddCategory(evt.Category);
                AddDate(evt.EventDate);
            }
        }

        public void ClearAllStructures()
        {
            _eventHistory.Clear();
            _eventQueue.Clear();
            _priorityQueue.Clear();
            _eventHashTable.Clear();
            _eventsByCategory.Clear();
            _eventsByDate.Clear();
            _uniqueCategories.Clear();
            _uniqueDates.Clear();
        }

        public void AddEventToHashTable(int key, LocalEvent eventItem)
        {
            _eventHashTable[key] = eventItem;
        }

        public LocalEvent? GetEventFromHashTable(int key)
        {
            return _eventHashTable.ContainsKey(key) ? _eventHashTable[key] : null;
        }

        public bool RemoveEventFromHashTable(int key)
        {
            return _eventHashTable.Remove(key);
        }

        public Dictionary<int, LocalEvent> GetHashTable()
        {
            return new Dictionary<int, LocalEvent>(_eventHashTable);
        }

        public void AddEventToDictionary(string category, LocalEvent eventItem)
        {
            if (!_eventsByCategory.ContainsKey(category))
            {
                _eventsByCategory[category] = new List<LocalEvent>();
            }
            _eventsByCategory[category].Add(eventItem);
        }

        public List<LocalEvent> GetEventsByCategory(string category)
        {
            return _eventsByCategory.ContainsKey(category)
                ? new List<LocalEvent>(_eventsByCategory[category])
                : new List<LocalEvent>();
        }

        public Dictionary<string, List<LocalEvent>> GetEventsByCategory()
        {
            return new Dictionary<string, List<LocalEvent>>(_eventsByCategory);
        }

        public Dictionary<string, List<LocalEvent>> GetEventDictionary()
        {
            return new Dictionary<string, List<LocalEvent>>(_eventsByCategory);
        }

        public void AddEventToSortedDictionary(DateTime date, LocalEvent eventItem)
        {
            var dateOnly = date.Date;
            if (!_eventsByDate.ContainsKey(dateOnly))
            {
                _eventsByDate[dateOnly] = new List<LocalEvent>();
            }
            _eventsByDate[dateOnly].Add(eventItem);
        }

        public List<LocalEvent> GetEventsByDate(DateTime date)
        {
            var dateOnly = date.Date;
            return _eventsByDate.ContainsKey(dateOnly)
                ? new List<LocalEvent>(_eventsByDate[dateOnly])
                : new List<LocalEvent>();
        }

        public SortedDictionary<DateTime, List<LocalEvent>> GetEventsByDate()
        {
            return new SortedDictionary<DateTime, List<LocalEvent>>(_eventsByDate);
        }

        public SortedDictionary<DateTime, List<LocalEvent>> GetSortedEventDictionary()
        {
            return new SortedDictionary<DateTime, List<LocalEvent>>(_eventsByDate);
        }

        public HashSet<string> GetUniqueCategories()
        {
            return new HashSet<string>(_uniqueCategories);
        }

        public HashSet<DateTime> GetUniqueDates()
        {
            return new HashSet<DateTime>(_uniqueDates);
        }

        public void AddCategory(string category)
        {
            _uniqueCategories.Add(category);
        }

        public void AddDate(DateTime date)
        {
            _uniqueDates.Add(date.Date);
        }

        public bool CategoryExists(string category)
        {
            return _uniqueCategories.Contains(category);
        }

        public bool DateExists(DateTime date)
        {
            return _uniqueDates.Contains(date.Date);
        }

        public void PushEventToHistory(LocalEvent eventItem)
        {
            _eventHistory.Push(eventItem);
        }

        public LocalEvent? PopEventFromHistory()
        {
            return _eventHistory.Count > 0 ? _eventHistory.Pop() : null;
        }

        public LocalEvent? PeekLastEvent()
        {
            return _eventHistory.Count > 0 ? _eventHistory.Peek() : null;
        }

        public Stack<LocalEvent> GetEventHistory()
        {
            return new Stack<LocalEvent>(_eventHistory.Reverse());
        }

        public void EnqueueEvent(LocalEvent eventItem)
        {
            _eventQueue.Enqueue(eventItem);
        }

        public LocalEvent? DequeueEvent()
        {
            return _eventQueue.Count > 0 ? _eventQueue.Dequeue() : null;
        }

        public LocalEvent? PeekNextEvent()
        {
            return _eventQueue.Count > 0 ? _eventQueue.Peek() : null;
        }

        public Queue<LocalEvent> GetEventQueue()
        {
            return new Queue<LocalEvent>(_eventQueue);
        }

        public void EnqueuePriorityEvent(LocalEvent eventItem, int priority)
        {
            if (!_priorityQueue.ContainsKey(priority))
            {
                _priorityQueue[priority] = new Queue<LocalEvent>();
            }
            _priorityQueue[priority].Enqueue(eventItem);
        }

        public LocalEvent? DequeuePriorityEvent()
        {
            if (_priorityQueue.Count == 0) return null;

            var highestPriority = _priorityQueue.First();
            var eventItem = highestPriority.Value.Dequeue();

            if (highestPriority.Value.Count == 0)
            {
                _priorityQueue.Remove(highestPriority.Key);
            }

            return eventItem;
        }

        public SortedDictionary<int, Queue<LocalEvent>> GetPriorityEvents()
        {
            return new SortedDictionary<int, Queue<LocalEvent>>(_priorityQueue);
        }

        public List<LocalEvent> GetEventsByPriority()
        {
            var result = new List<LocalEvent>();

            foreach (var priorityLevel in _priorityQueue.OrderBy(p => p.Key))
            {
                result.AddRange(priorityLevel.Value);
            }

            return result;
        }
    }
}
