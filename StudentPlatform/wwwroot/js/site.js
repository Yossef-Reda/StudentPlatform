// StudentPlatform — shared UI behaviours (vanilla JS, no build step)
(function () {
  'use strict';

  /* ---------- Dark mode (localStorage + data-bs-theme) ---------- */
  var THEME_KEY = 'sp-theme';
  function applyTheme(t) {
    document.documentElement.setAttribute('data-bs-theme', t);
    syncThemeIcons(t);
  }
  function syncThemeIcons(t) {
    document.querySelectorAll('[data-theme-icon]').forEach(function (el) {
      el.className = t === 'dark' ? 'bi bi-sun-fill' : 'bi bi-moon-stars-fill';
    });
    document.querySelectorAll('[data-theme-toggle]').forEach(function (btn) {
      btn.setAttribute('aria-label', t === 'dark' ? 'التبديل إلى الوضع الفاتح' : 'التبديل إلى الوضع الداكن');
      btn.setAttribute('title', t === 'dark' ? 'الوضع الفاتح' : 'الوضع الداكن');
    });
  }
  function initTheme() {
    var saved = null;
    try { saved = localStorage.getItem(THEME_KEY); } catch (e) { /* ignore */ }
    if (saved !== 'light' && saved !== 'dark') {
      saved = (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) ? 'dark' : 'light';
    }
    applyTheme(saved);
    document.querySelectorAll('[data-theme-toggle]').forEach(function (btn) {
      btn.addEventListener('click', function () {
        var cur = document.documentElement.getAttribute('data-bs-theme') === 'dark' ? 'dark' : 'light';
        var next = cur === 'dark' ? 'light' : 'dark';
        try { localStorage.setItem(THEME_KEY, next); } catch (e) { /* ignore */ }
        applyTheme(next);
      });
    });
  }

  /* ---------- Toasts from TempData (server renders .sp-toast) ---------- */
  function initToasts() {
    var nodes = document.querySelectorAll('.sp-toast');
    if (!nodes.length || typeof bootstrap === 'undefined') return;
    nodes.forEach(function (el, i) {
      var t = new bootstrap.Toast(el, { delay: 4500 });
      setTimeout(function () { t.show(); }, 120 * i);
    });
  }

  /* ---------- Student page: filter tabs (All/Pending/Submitted/Overdue) ---------- */
  function initFilters() {
    var btns = document.querySelectorAll('[data-filter-btn]');
    if (!btns.length) return;
    var cards = document.querySelectorAll('[data-lecture-card]');
    var empty = document.getElementById('spNoResult');
    btns.forEach(function (btn) {
      btn.addEventListener('click', function () {
        btns.forEach(function (b) { b.classList.remove('active'); b.setAttribute('aria-selected', 'false'); });
        btn.classList.add('active');
        btn.setAttribute('aria-selected', 'true');
        var f = btn.getAttribute('data-filter-btn');
        var visibleLectures = 0;
        cards.forEach(function (card) {
          var rows = card.querySelectorAll('[data-status]');
          var shown = 0;
          rows.forEach(function (r) {
            var show = (f === 'all') || (r.getAttribute('data-status') === f);
            r.style.display = show ? '' : 'none';
            if (show) shown++;
          });
          var listEmpty = card.querySelector('[data-empty-assignments]');
          // lecture with zero assignments stays visible only on "all"
          var hasAssignments = rows.length > 0;
          var visible = hasAssignments ? shown > 0 : (f === 'all');
          card.style.display = visible ? '' : 'none';
          if (visible) visibleLectures++;
          if (listEmpty) listEmpty.style.display = (!hasAssignments && f === 'all') ? '' : 'none';
          var perLectureEmpty = card.querySelector('[data-filter-empty]');
          if (perLectureEmpty) perLectureEmpty.style.display = (hasAssignments && shown === 0) ? '' : 'none';
        });
        if (empty) empty.style.display = visibleLectures === 0 ? '' : 'none';
      });
    });
  }

  /* ---------- Countdown / remaining-time text ---------- */
  function fmtRem(ms) {
    if (ms <= 0) return 'انتهى الموعد';
    var s = Math.floor(ms / 1000);
    var d = Math.floor(s / 86400);
    var h = Math.floor((s % 86400) / 3600);
    var m = Math.floor((s % 3600) / 60);
    if (d > 0) return 'متبقٍ ' + d + ' يوم و ' + h + ' ساعة';
    if (h > 0) return 'متبقٍ ' + h + ' ساعة و ' + m + ' دقيقة';
    if (m > 0) return 'متبقٍ ' + m + ' دقيقة';
    return 'متبقٍ أقل من دقيقة';
  }
  function initCountdowns() {
    var nodes = document.querySelectorAll('[data-deadline]');
    if (!nodes.length) return;
    function tick() {
      var now = Date.now();
      nodes.forEach(function (el) {
        var ts = Date.parse(el.getAttribute('data-deadline'));
        if (isNaN(ts)) return;
        var diff = ts - now;
        el.textContent = fmtRem(diff);
        el.classList.toggle('urgent', diff > 0 && diff < 24 * 3600 * 1000);
      });
    }
    tick();
    setInterval(tick, 60000);
  }

  /* ---------- Drag & drop file zones ---------- */
  function initDropzones() {
    document.querySelectorAll('[data-dropzone]').forEach(function (zone) {
      var input = zone.querySelector('input[type="file"]');
      var label = zone.querySelector('[data-file-name]');
      if (!input) return;
      zone.addEventListener('click', function (e) {
        if (e.target !== input) input.click();
      });
      zone.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); input.click(); }
      });
      ['dragenter', 'dragover'].forEach(function (ev) {
        zone.addEventListener(ev, function (e) { e.preventDefault(); zone.classList.add('dragover'); });
      });
      ['dragleave', 'drop'].forEach(function (ev) {
        zone.addEventListener(ev, function (e) { e.preventDefault(); zone.classList.remove('dragover'); });
      });
      zone.addEventListener('drop', function (e) {
        if (e.dataTransfer && e.dataTransfer.files && e.dataTransfer.files.length) {
          input.files = e.dataTransfer.files;
          input.dispatchEvent(new Event('change', { bubbles: true }));
        }
      });
      input.addEventListener('change', function () {
        var name = input.files && input.files.length ? input.files[0].name : 'اختر ملف الحل أو اسحبه هنا (PDF / صور / مضغوط)';
        if (label) label.textContent = name;
        zone.classList.toggle('has-file', !!(input.files && input.files.length));
      });
    });
  }

  /* ---------- Teacher tables: client-side search ---------- */
  function initTableSearch() {
    var box = document.querySelector('[data-table-search]');
    if (!box) return;
    var target = document.querySelector(box.getAttribute('data-table-search'));
    if (!target) return;
    box.addEventListener('input', function () {
      var q = box.value.trim();
      target.querySelectorAll('tbody tr').forEach(function (tr) {
        tr.style.display = (!q || tr.textContent.indexOf(q) !== -1) ? '' : 'none';
      });
    });
  }

  /* ---------- Delete confirmation modal (generic) ---------- */
  var deleteTarget = null;
  function initDeleteModal() {
    var modal = document.getElementById('spDeleteModal');
    if (!modal) return;
    modal.addEventListener('show.bs.modal', function (e) {
      var btn = e.relatedTarget;
      deleteTarget = btn ? btn.getAttribute('data-delete-name') : null;
      var nameEl = modal.querySelector('[data-delete-item-name]');
      if (nameEl && deleteTarget) nameEl.textContent = deleteTarget;
    });
  }

  document.addEventListener('DOMContentLoaded', function () {
    initTheme();
    initToasts();
    initFilters();
    initCountdowns();
    initDropzones();
    initTableSearch();
    initDeleteModal();
  });
})();
