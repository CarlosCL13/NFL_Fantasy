export interface League {
  leagueId?: number;
  name: string;
  description?: string;
  maxTeams: number;
  createdAt?: Date;
  status?: string;
  seasonId?: number;
  commissionerId?: number;
  playoffType: number;
  allowDecimalPoints?: boolean;
  defaultPositions?: string;
  defaultScoring?: string;
  tradeDeadlineActive?: boolean;
  maxTradesPerTeam?: number;
  maxFreeAgentsPerTeam?: number;
  isActive?: boolean;
  teams?: Team[];
}

export interface Team {
  teamId?: number;
  teamName: string;
  alias: string;
  userId?: number;
  leagueId?: number;
  createdAt?: Date;
}

export interface CreateLeagueDto {
  name: string;
  description?: string;
  maxTeams: number;
  password: string;
  playoffType: number;
  commissionerTeamName: string;
  commissionerAlias: string;
}

export interface CreateLeagueResponse {
  message: string;
  leagueId: number;
  remainingSpots: number;
}

export interface NameAvailabilityResponse {
  isAvailable: boolean;
  message: string;
}