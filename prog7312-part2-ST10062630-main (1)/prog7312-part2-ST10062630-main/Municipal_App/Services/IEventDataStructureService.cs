using Municipal_App.Models;
using System;
using System.Collections.Generic;

namespace Municipal_App.Services
{
    public interface IEventDataStructureService
    {
        // Initialization
        void LoadEvents(List<LocalEvent> events);
        void InitializeWithEvents(List<LocalEvent> events);
        void ClearAllStructures();

        // Hash Table operations
        void AddEventToHashTable(int key, LocalEvent eventItem);
        LocalEvent? GetEventFromHashTable(int key);
        bool RemoveEventFromHashTable(int key);
        Dictionary<int, LocalEvent> GetHashTable();

        // Dictionary operations
        void AddEventToDictionary(string category, LocalEvent eventItem);
        List<LocalEvent> GetEventsByCategory(string category);
        Dictionary<string, List<LocalEvent>> GetEventDictionary();
        Dictionary<string, List<LocalEvent>> GetEventsByCategory();

        // Sorted Dictionary operations
        void AddEventToSortedDictionary(DateTime date, LocalEvent eventItem);
        List<LocalEvent> GetEventsByDate(DateTime date);
        SortedDictionary<DateTime, List<LocalEvent>> GetSortedEventDictionary();
        SortedDictionary<DateTime, List<LocalEvent>> GetEventsByDate();

        // Set operations
        HashSet<string> GetUniqueCategories();
        HashSet<DateTime> GetUniqueDates();
        void AddCategory(string category);
        void AddDate(DateTime date);
        bool CategoryExists(string category);
        bool DateExists(DateTime date);

        // Stack operations
        void PushEventToHistory(LocalEvent eventItem);
        LocalEvent? PopEventFromHistory();
        LocalEvent? PeekLastEvent();
        Stack<LocalEvent> GetEventHistory();

        // Queue operations
        void EnqueueEvent(LocalEvent eventItem);
        LocalEvent? DequeueEvent();
        LocalEvent? PeekNextEvent();
        Queue<LocalEvent> GetEventQueue();

        // Priority Queue operations
        void EnqueuePriorityEvent(LocalEvent eventItem, int priority);
        LocalEvent? DequeuePriorityEvent();
        SortedDictionary<int, Queue<LocalEvent>> GetPriorityEvents();
        List<LocalEvent> GetEventsByPriority();
    }
}
