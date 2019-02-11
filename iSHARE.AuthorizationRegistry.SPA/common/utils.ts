export const Utils = {
  trim: function(date: Date): Date {
    if (date == null) {
      return null;
    }
    return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
  }
};
