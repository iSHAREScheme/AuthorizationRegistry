export class Query {
  sortBy: string;
  sortOrder: string;
  filter: string;
  pageSize: number;
  page: number;
  total: number;

  constructor(sortBy: string) {
    this.sortBy = sortBy;
    this.sortOrder = 'asc';
    this.filter = '';
    this.pageSize = 10;
    this.page = 1;
    this.total = 0;
  }
}
