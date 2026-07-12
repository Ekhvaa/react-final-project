export const formatDate = (dateInput) => {
  return dateInput ? new Date(dateInput).toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' }) : '';
};