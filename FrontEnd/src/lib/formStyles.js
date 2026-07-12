export const inputClass = (hasError) =>
    `w-full rounded-lg border px-4 py-2.5 focus:outline-none focus:ring-2 focus:border-transparent transition-shadow ${
        hasError ? 'border-red-400 focus:ring-red-300' : 'border-slate-300 focus:ring-[#8dfc9c]'
    }`;