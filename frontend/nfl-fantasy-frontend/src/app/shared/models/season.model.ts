export interface Season {
  seasonId?: number;
  name: string;
  weeksCount: number;
  startDate: Date;
  endDate: Date;
  isCurrent: boolean;
  createdAt?: Date;
  weeks?: Week[];
}

export interface Week {
  weekId?: number;
  seasonId?: number;
  number: number;
  startDate: Date;
  endDate: Date;
}

export interface CreateSeasonDto {
  name: string;
  weeksCount: number;
  startDate: Date;
  endDate: Date;
  isCurrent: boolean;
}