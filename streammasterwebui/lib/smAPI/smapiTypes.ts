export interface StationChannelName
{
	Channel: string;
	ChannelName: string;
	DisplayName: string;
	EPGNumber: number;
	Id: string;
	Logo: string;
}
export interface QueryStringParameters
{
	JSONArgumentString?: string;
	JSONFiltersString?: string;
	OrderBy: string;
	PageNumber: number;
	PageSize: number;
}
export interface SMChannelStreamRankRequest
{
	Rank: number;
	SMChannelId: number;
	SMStreamId: string;
}
export interface SMChannelChannelRankRequest
{
	ChildSMChannelId: number;
	ParentSMChannelId: number;
	Rank: number;
}
export interface FieldData
{
	Entity: string;
	Field: string;
	Id: string;
	Value: any;
}
export interface ImageDownloadServiceStatus
{
	Id: number;
	Logos: DownloadStats;
	ProgramLogos: DownloadStats;
}
export interface DownloadStats
{
	AlreadyExists: number;
	Attempts: number;
	Errors: number;
	Queue: number;
	Successful: number;
}
export interface SMMessage
{
	Detail?: string;
	Severity: string;
	Summary: string;
}
export interface SMStreamInfo
{
	ClientUserAgent?: string;
	CommandProfile: CommandProfileDto;
	Id: string;
	Name: string;
	SecondsIn: number;
	SMStreamType: SMStreamTypeEnum;
	Url: string;
}
export interface SMTask
{
	Command: string;
	Id: number;
	IsRunning: boolean;
	QueueTS: any;
	StartTS: any;
	Status: string;
	StopTS: any;
}
export interface StationIdLineup
{
	Id: string;
	Lineup: string;
	StationId: string;
}
export interface StreamGroupProfile
{
	CommandProfileName: string;
	Id: number;
	OutputProfileName: string;
	ProfileName: string;
	StreamGroupId: number;
}
export interface StreamGroupProfileDto
{
	CommandProfileName: string;
	HDHRLink: string;
	Id: number;
	M3ULink: string;
	OutputProfileName: string;
	ProfileName: string;
	ShortEPGLink: string;
	ShortHDHRLink: string;
	ShortM3ULink: string;
	StreamGroupId: number;
	XMLLink: string;
	Mapping(profile: any) : void;
}
export interface StreamGroupProfileLinks
{
	CommandProfileName: string;
	HDHRLink: string;
	Id: number;
	M3ULink: string;
	OutputProfileName: string;
	ProfileName: string;
	StreamGroupId: number;
	XMLLink: string;
}
export interface StreamGroupSMChannelLink
{
	IsReadOnly: boolean;
	Rank: number;
	SMChannel: any;
	SMChannelId: number;
	StreamGroup: any;
	StreamGroupId: number;
}
export interface VideoInfo
{
	Created: any;
	JsonOutput: string;
	Key: string;
	StreamId: string;
	StreamName: string;
}
export interface VideoInfoDto
{
	Created: any;
	JsonOutput: string;
	Key: string;
	StreamId: string;
	StreamName: string;
}
export interface ChannelGroupDto
{
	ActiveCount: number;
	APIName: string;
	HiddenCount: number;
	Id: number;
	IsHidden: boolean;
	IsReadOnly: boolean;
	IsSystem: boolean;
	Name: string;
	RegexMatch: string;
	TotalCount: number;
}
export interface EPGColorDto
{
	Color: string;
	EPGNumber: number;
	Id: number;
	StationId: string;
}
export interface EPGFileDto
{
	AutoUpdate: boolean;
	ChannelCount: number;
	Color: string;
	Description: string;
	DownloadErrors: number;
	EPGNumber: number;
	EPGStartDate: any;
	EPGStopDate: any;
	HoursToUpdate: number;
	Id: number;
	LastDownloadAttempt: any;
	LastDownloaded: any;
	Name: string;
	NeedsUpdate: boolean;
	ProgrammeCount: number;
	Source: string;
	TimeShift: number;
	Url: string;
}
export interface EPGFilePreviewDto
{
	ChannelLogo: string;
	ChannelName: string;
	Id: string;
}
export interface LogoDto
{
	ContentType: string;
	FileName: string;
	Image: number[];
	Url: string;
}
export interface LogoInfo
{
	Ext: string;
	FileName: string;
	FullPath: string;
	Id: string;
	IsSchedulesDirect: boolean;
	IsSVG: boolean;
	Name: string;
	SMFileType: SMFileTypes;
	Url: string;
}
export interface M3UFileDto
{
	AutoSetChannelNumbers: boolean;
	AutoUpdate: boolean;
	DefaultStreamGroupName?: string;
	Description: string;
	DownloadErrors: number;
	HoursToUpdate: number;
	Id: number;
	LastDownloadAttempt: any;
	LastDownloaded: any;
	M3U8OutPutProfile: string;
	M3UKey: M3UKey;
	M3UName: M3UField;
	MaxStreamCount: number;
	Name: string;
	NeedsUpdate: boolean;
	Source: string;
	StartingChannelNumber: number;
	StreamCount: number;
	SyncChannels: boolean;
	Url: string;
	VODTags: string[];
}
export interface SDSystemStatus
{
	IsSystemReady: boolean;
}
export interface SettingDto
{
	AdminPassword: string;
	AdminUserName: string;
	AppendChannelName: boolean;
	AuthenticationMethod: string;
	AutoSetEPG: boolean;
	BackupEnabled: boolean;
	BackupInterval: number;
	BackupVersionsToKeep: number;
	CleanURLs: boolean;
	ClientReadTimeOutSeconds: number;
	ClientUserAgent: string;
	DefaultCommandProfileName: string;
	DefaultCompression: string;
	DefaultLogo: string;
	DefaultOutputProfileName: string;
	DeviceID: string;
	EnableDBDebug: boolean;
	EnableSSL: boolean;
	FFMPegExecutable: string;
	FFProbeExecutable: string;
	GlobalStreamLimit: number;
	IconCacheExpirationDays: number;
	IsDebug: boolean;
	LogoCache: boolean;
	M3U8OutPutProfile: string;
	MaxConcurrentDownloads: number;
	MaxConnectRetry: number;
	MaxConnectRetryTimeMS: number;
	MaxLogFiles: number;
	MaxLogFileSizeMB: number;
	MaxStreamReStart: number;
	NameRegex?: string[];
	PrettyEPG: boolean;
	Release: string;
	SDSettings: SDSettings;
	ShowClientHostNames: boolean;
	ShowIntros: string;
	ShowMessageVideos: boolean;
	ShutDownDelay: number;
	SSLCertPassword: string;
	SSLCertPath: string;
	STRMBaseURL: string;
	UiFolder: string;
	UrlBase: string;
	Version: string;
	VideoStreamAlwaysUseEPGLogo: boolean;
}
export interface SMChannelDto
{
	APIName: string;
	BaseStreamID: string;
	ChannelId: string;
	ChannelName: string;
	ChannelNumber: number;
	ClientUserAgent?: string;
	CommandProfileName: string;
	CurrentRank: number;
	EPGId: string;
	Group: string;
	GroupTitle: string;
	Id: number;
	IsHidden: boolean;
	IsSystem: boolean;
	Logo: string;
	M3UFileId: number;
	Name: string;
	Rank: number;
	SMChannelDtos: SMChannelDto[];
	SMChannelType: SMChannelTypeEnum;
	SMStreamDtos: SMStreamDto[];
	StationId: string;
	StreamGroupIds: number[];
	StreamUrl: string;
	TimeShift: number;
	TVGName: string;
}
export interface SMStreamDto
{
	APIName: string;
	ChannelId: string;
	ChannelMembership: LogoInfo[];
	ChannelName: string;
	ChannelNumber: number;
	ClientUserAgent?: string;
	CommandProfileName?: string;
	CUID: string;
	EPGID: string;
	ExtInf?: string;
	FilePosition: number;
	Group: string;
	Id: string;
	IsHidden: boolean;
	IsSystem: boolean;
	IsUserCreated: boolean;
	Logo: string;
	M3UFileId: number;
	M3UFileName: string;
	Name: string;
	NeedsDelete: boolean;
	Rank: number;
	RealUrl: string;
	SMStreamType: SMStreamTypeEnum;
	StationId: string;
	TVGName: string;
	Url: string;
}
export interface StreamGroupDto
{
	AutoSetChannelNumbers: boolean;
	ChannelCount: number;
	CreateSTRM: boolean;
	DeviceID: string;
	GroupKey: string;
	HDHRLink: string;
	Id: number;
	IgnoreExistingChannelNumbers: boolean;
	IsReadOnly: boolean;
	IsSystem: boolean;
	M3ULink: string;
	Name: string;
	ShortEPGLink: string;
	ShortHDHRLink: string;
	ShortM3ULink: string;
	ShowIntros: number;
	SMChannels: StreamGroupSMChannelLink[];
	StartingChannelNumber: number;
	StreamGroupProfiles: StreamGroupProfileDto[];
	XMLLink: string;
}
export interface CommandProfile
{
	Command: string;
	IsReadOnly: boolean;
	Parameters: string;
}
export interface CommandProfileDto
{
	Command: string;
	IsReadOnly: boolean;
	Parameters: string;
	ProfileName: string;
}
export interface CustomLogo
{
	APIName: string;
	FileId: number;
	IsReadOnly: boolean;
	Name: string;
	Value: string;
}
export interface CustomLogoDto
{
	APIName: string;
	FileId: number;
	IsReadOnly: boolean;
	Name: string;
	Source: string;
	Value: string;
}
export interface CustomLogoRequest
{
	Name: string;
	Source: string;
}
export interface HLSSettings
{
	HLSFFMPEGOptions: string;
	HLSM3U8CreationTimeOutInSeconds: number;
	HLSM3U8Enable: boolean;
	HLSM3U8ReadTimeOutInSeconds: number;
	HLSReconnectDurationInSeconds: number;
	HLSSegmentCount: number;
	HLSSegmentDurationInSeconds: number;
	HLSTSReadTimeOutInSeconds: number;
}
export interface OutputProfile
{
	APIName: string;
	EnableChannelNumber: boolean;
	EnableGroupTitle: boolean;
	EnableIcon: boolean;
	Group: string;
	Id: string;
	IsReadOnly: boolean;
	Name: string;
}
export interface OutputProfileDto
{
	APIName: string;
	EnableChannelNumber: boolean;
	EnableGroupTitle: boolean;
	EnableIcon: boolean;
	Group: string;
	Id: string;
	IsReadOnly: boolean;
	Name: string;
	ProfileName: string;
}
export interface SDSettings
{
	AlternateLogoStyle: string;
	AlternateSEFormat: boolean;
	AppendEpisodeDesc: boolean;
	ArtworkSize: string;
	EpisodeAppendProgramDescription: boolean;
	EpisodeImages: boolean;
	ExcludeCastAndCrew: boolean;
	HeadendsToView: any[];
	MaxSubscribedLineups: number;
	MovieImages: boolean;
	MoviePosterAspect: string;
	PreferredLogoStyle: string;
	PrefixEpisodeDescription: boolean;
	PrefixEpisodeTitle: boolean;
	SDCountry: string;
	SDEnabled: boolean;
	SDEPGDays: number;
	SDPassword: string;
	SDPostalCode: string;
	SDStationIds: StationIdLineup[];
	SDTooManyRequestsSuspend: any;
	SDUserName: string;
	SeasonImages: boolean;
	SeriesImages: boolean;
	SeriesPosterAspect: string;
	SportsImages: boolean;
	XmltvAddFillerData: boolean;
	XmltvExtendedInfoInTitleDescriptions: boolean;
	XmltvFillerProgramLength: number;
	XmltvIncludeChannelNumbers: boolean;
	XmltvSingleImage: boolean;
}
export interface SDSettingsRequest
{
	AlternateLogoStyle?: string;
	AlternateSEFormat?: boolean;
	AppendEpisodeDesc?: boolean;
	ArtworkSize?: string;
	ExcludeCastAndCrew?: boolean;
	HeadendsToView?: any[];
	MaxSubscribedLineups?: number;
	PreferredLogoStyle?: string;
	PrefixEpisodeDescription?: boolean;
	PrefixEpisodeTitle?: boolean;
	SDCountry?: string;
	SDEnabled?: boolean;
	SDEPGDays?: number;
	SDPassword?: string;
	SDPostalCode?: string;
	SDStationIds?: StationIdLineup[];
	SDUserName?: string;
	SeasonImages?: boolean;
	SeriesImages?: boolean;
	SeriesPosterAspect?: string;
	SportsImages?: boolean;
	XmltvAddFillerData?: boolean;
	XmltvExtendedInfoInTitleDescriptions?: boolean;
	XmltvFillerProgramLength?: number;
	XmltvIncludeChannelNumbers?: boolean;
	XmltvSingleImage?: boolean;
}
export interface UpdateSettingResponse
{
	NeedsLogOut: boolean;
	Settings?: SettingDto;
}
export interface APIResponse
{
	ErrorMessage?: string;
	IsError: boolean;
	Message?: string;
	NotFound: APIResponse;
}
export interface DataResponse<T>
{
	Count: number;
	Data?: T;
	ErrorMessage?: string;
	IsError: boolean;
	Message?: string;
	NotFound: DataResponse<T>;
	Ok: DataResponse<T>;
	totalItemCount?: number;
	TotalItemCount: number;
}
export interface NoClass
{
}
export interface PagedResponse<T>
{
	Count: number;
	Data?: T[];
	ErrorMessage?: string;
	First: number;
	IsError: boolean;
	Message?: string;
	NotFound: DataResponse<T[]>;
	Ok: DataResponse<T[]>;
	PageNumber: number;
	PageSize: number;
	totalItemCount?: number;
	TotalItemCount: number;
	TotalPageCount: number;
}
export interface CountryData
{
	Countries?: Country[];
	Id?: string;
	Key?: string;
}
export interface LineupResult
{
	Map?: Map[];
	Metadata?: Metadata;
	Stations: Station[];
}
export interface Logo
{
	Category: string;
	Height: number;
	Md5?: string;
	Source: string;
	Url: string;
	Width: number;
}
export interface Map
{
	AtscMajor?: number;
	AtscMinor?: number;
	Channel: string;
	StationId?: string;
	UhfVhf?: number;
}
export interface Metadata
{
	Lineup?: string;
	Modified?: any;
	Modulation: string;
	Transport?: string;
}
export interface Station
{
	Affiliate?: string;
	Broadcaster?: any;
	BroadcastLanguage?: string[];
	Callsign?: string;
	Country?: string;
	DescriptionLanguage?: string[];
	IsCommercialFree?: boolean;
	Lineup: string;
	Logo?: Logo;
	Name?: string;
	PostalCode?: string;
	StationId?: string;
	StationLogos: Logo[];
}
export interface StationPreview
{
	Affiliate?: string;
	Callsign?: string;
	Country?: string;
	Id?: string;
	Lineup?: string;
	Logo?: Logo;
	Name?: string;
	PostalCode?: string;
	StationId?: string;
}
export interface Country
{
	FullName: string;
	OnePostalCode: boolean;
	PostalCode: string;
	PostalCodeExample: string;
	ShortName: string;
}
export interface BaseResponse
{
	Code: number;
	Datetime: any;
	Message: string;
	Response: string;
	ServerId: string;
	Uuid: string;
}
export interface LineupChannel
{
	AtscMajor?: number;
	AtscMinor?: number;
	AtscType: string;
	Channel: string;
	ChannelMajor?: number;
	ChannelMinor?: number;
	ChannelNumber: string;
	DeliverySystem: string;
	Fec: string;
	FrequencyHz: number;
	LogicalChannelNumber: string;
	MatchName: string;
	MatchType: string;
	ModulationSystem: string;
	MyChannelNumber: number;
	MyChannelSubnumber: number;
	NetworkId: number;
	Polarization: string;
	ProviderCallsign: string;
	ProviderChannel: string;
	ServiceId: number;
	StationId: string;
	Symbolrate: number;
	TransportId: number;
	UhfVhf?: number;
	VirtualChannel: string;
}
export interface LineupPreviewChannel
{
	Affiliate: string;
	Callsign: string;
	Channel: string;
	Id: number;
	Name: string;
}
export interface LineupResponse
{
	Code: number;
	Datetime: any;
	Lineups: SubscribedLineup[];
	Message: string;
	Response: string;
	ServerId: string;
	Uuid: string;
}
export interface StationBroadcaster
{
	City: string;
	Country: string;
	Postalcode: string;
	State: string;
}
export interface SubscribedLineup
{
	Id: string;
	IsDeleted: boolean;
	Lineup: string;
	Location: string;
	Name: string;
	Transport: string;
	Uri: string;
}
export interface HeadendDto
{
	Country: string;
	HeadendId: string;
	Id: string;
	Lineup: string;
	Location: string;
	Name: string;
	PostCode: string;
	Transport: string;
}
export interface GetVsRequest
{
	StreamGroupId?: number;
	StreamGroupProfileId?: number;
}
export interface V
{
	BaseUrl: string;
	DefaultRealUrl: string;
	Id: number;
	Name: string;
	RealUrl: string;
	StreamGroupId: number;
	StreamGroupName: string;
	StreamGroupProfileId: number;
	StreamGroupProfileName: string;
}
export interface CancelAllChannelsRequest
{
}
export interface CancelChannelRequest
{
	SMChannelId: number;
}
export interface CancelClientRequest
{
	UniqueRequestId: string;
}
export interface MoveToNextStreamRequest
{
	SMChannelId: number;
}
export interface GetPagedStreamGroupsRequest
{
	Parameters: QueryStringParameters;
}
export interface GetStreamGroupProfilesRequest
{
}
export interface GetStreamGroupRequest
{
	SGName: string;
}
export interface GetStreamGroupsRequest
{
}
export interface AddProfileToStreamGroupRequest
{
	CommandProfileName: string;
	OutputProfileName: string;
	ProfileName: string;
	StreamGroupId: number;
}
export interface CreateStreamGroupRequest
{
	CommandProfileName?: string;
	CreateSTRM?: boolean;
	GroupKey?: string;
	Name: string;
	OutputProfileName?: string;
}
export interface DeleteStreamGroupRequest
{
	StreamGroupId: number;
}
export interface RemoveStreamGroupProfileRequest
{
	ProfileName: string;
	StreamGroupId: number;
}
export interface UpdateStreamGroupProfileRequest
{
	CommandProfileName?: string;
	NewProfileName: string;
	OutputProfileName?: string;
	ProfileName: string;
	StreamGroupId: number;
}
export interface UpdateStreamGroupRequest
{
	CreateSTRM?: boolean;
	DeviceID?: string;
	GroupKey?: string;
	NewName?: string;
	StreamGroupId: number;
}
export interface GetStreamGroupSMChannelsRequest
{
	StreamGroupId: number;
}
export interface AddSMChannelsToStreamGroupByParametersRequest
{
	Parameters: QueryStringParameters;
	StreamGroupId: number;
}
export interface AddSMChannelsToStreamGroupRequest
{
	SMChannelIds: number[];
	StreamGroupId: number;
}
export interface AddSMChannelToStreamGroupRequest
{
	SMChannelId: number;
	StreamGroupId: number;
}
export interface RemoveSMChannelFromStreamGroupRequest
{
	SMChannelId: number;
	StreamGroupId: number;
}
export interface GetChannelMetricsRequest
{
}
export interface GetVideoInfoRequest
{
	SMStreamId: string;
}
export interface GetVideoInfosRequest
{
}
export interface GetSMTasksRequest
{
}
export interface SendSMTasksRequest
{
	SMTasks: SMTask[];
}
export interface GetPagedSMStreamsRequest
{
	Parameters: QueryStringParameters;
}
export interface CreateSMStreamRequest
{
	ChannelNumber?: number;
	CommandProfileName?: string;
	EPGID?: string;
	Group?: string;
	Logo?: string;
	Name: string;
	Url: string;
}
export interface DeleteSMStreamRequest
{
	SMStreamId: string;
}
export interface SetSMStreamsVisibleByIdRequest
{
	Ids: string[];
	IsHidden: boolean;
}
export interface ClearByTag
{
	Entity: string;
	Tag: string;
}
export interface ToggleSMStreamsVisibleByIdRequest
{
	Ids: string[];
}
export interface ToggleSMStreamVisibleByIdRequest
{
	Id: string;
}
export interface ToggleSMStreamVisibleByParametersRequest
{
	Parameters: QueryStringParameters;
}
export interface UpdateSMStreamRequest
{
	ChannelNumber?: number;
	CommandProfileName?: string;
	EPGID?: string;
	Group?: string;
	Logo?: string;
	Name?: string;
	SMStreamId: string;
	Url: string;
}
export interface SendSMErrorRequest
{
	Detail: string;
	Summary: string;
}
export interface SendSMInfoRequest
{
	Detail: string;
	Summary: string;
}
export interface SendSMMessageRequest
{
	Message: SMMessage;
}
export interface SendSMWarnRequest
{
	Detail: string;
	Summary: string;
}
export interface SendSuccessRequest
{
	Detail: string;
	Summary: string;
}
export interface GetPagedSMChannelsRequest
{
	Parameters: QueryStringParameters;
}
export interface GetSMChannelNamesRequest
{
}
export interface GetVideoStreamNamesAndUrlsRequest
{
}
export interface IdNameUrl
{
	Id: number;
	Name: string;
	Url: string;
}
export interface AutoSetEPGFromParametersRequest
{
	Parameters: QueryStringParameters;
}
export interface AutoSetEPGRequest
{
	Ids: number[];
}
export interface AutoSetSMChannelNumbersFromParametersRequest
{
	OverwriteExisting?: boolean;
	Parameters: QueryStringParameters;
	StartingNumber?: number;
	StreamGroupId: number;
}
export interface AutoSetSMChannelNumbersRequest
{
	OverwriteExisting?: boolean;
	SMChannelIds: number[];
	StartingNumber?: number;
	StreamGroupId: number;
}
export interface CopySMChannelRequest
{
	NewName: string;
	SMChannelId: number;
}
export interface CreateMultiViewChannelRequest
{
	ChannelNumber?: number;
	EPGId?: string;
	Group?: string;
	Logo?: string;
	Name: string;
	SMSChannelIds?: number[];
	StreamGroup?: string;
}
export interface CreateSMChannelRequest
{
	ChannelNumber?: number;
	ClientUserAgent?: string;
	CommandProfileName?: string;
	EPGId?: string;
	Group?: string;
	Logo?: string;
	Name: string;
	SMStreamsIds?: string[];
	StationId?: string;
	TimeShift?: number;
}
export interface CreateSMChannelsFromStreamParametersRequest
{
	DefaultStreamGroupName?: string;
	M3UFileId: number;
	Parameters: QueryStringParameters;
	StreamGroupId?: number;
}
export interface CreateSMChannelsFromStreamsRequest
{
	StreamGroupId?: number;
	StreamIds: string[];
}
export interface DeleteSMChannelRequest
{
	SMChannelId: number;
}
export interface DeleteSMChannelsFromParametersRequest
{
	Parameters: QueryStringParameters;
}
export interface DeleteSMChannelsRequest
{
	SMChannelIds: number[];
}
export interface SetSMChannelEPGIdRequest
{
	EPGId: string;
	SMChannelId: number;
}
export interface SetSMChannelGroupRequest
{
	Group: string;
	SMChannelId: number;
}
export interface SetSMChannelLogoRequest
{
	Logo: string;
	SMChannelId: number;
}
export interface SetSMChannelNameRequest
{
	Name: string;
	SMChannelId: number;
}
export interface SetSMChannelNumberRequest
{
	ChannelNumber: number;
	SMChannelId: number;
}
export interface SetSMChannelsGroupFromParametersRequest
{
	Group: string;
	Parameters: QueryStringParameters;
}
export interface SetSMChannelsGroupRequest
{
	Group: string;
	SMChannelIds: number[];
}
export interface SetSMChannelsLogoFromEPGFromParametersRequest
{
	Parameters: QueryStringParameters;
}
export interface SetSMChannelsLogoFromEPGRequest
{
	Ids: number[];
}
export interface SetSMChannelsCommandProfileNameFromParametersRequest
{
	CommandProfileName: string;
	Parameters: QueryStringParameters;
}
export interface SetSMChannelsCommandProfileNameRequest
{
	CommandProfileName: string;
	SMChannelIds: number[];
}
export interface SetSMChannelCommandProfileNameRequest
{
	CommandProfileName: string;
	SMChannelId: number;
}
export interface ToggleSMChannelsVisibleByIdRequest
{
	Ids: number[];
}
export interface ToggleSMChannelVisibleByIdRequest
{
	Id: number;
}
export interface ToggleSMChannelVisibleByParametersRequest
{
	Parameters: QueryStringParameters;
}
export interface UpdateMultiViewChannelRequest
{
	ChannelNumber?: number;
	EPGId?: string;
	Group?: string;
	Id: number;
	Logo?: string;
	Name?: string;
	SMChannelIds?: number[];
}
export interface UpdateSMChannelRequest
{
	ChannelNumber?: number;
	ClientUserAgent?: string;
	CommandProfileName?: string;
	EPGId?: string;
	Group?: string;
	Id: number;
	Logo?: string;
	Name?: string;
	SMStreamsIds?: string[];
	StationId?: string;
	TimeShift?: number;
	VideoStreamHandler?: VideoStreamHandlers;
}
export interface GetSMChannelStreamsRequest
{
	SMChannelId: number;
}
export interface AddSMStreamToSMChannelRequest
{
	Rank?: number;
	SMChannelId: number;
	SMStreamId: string;
}
export interface RemoveSMStreamFromSMChannelRequest
{
	SMChannelId: number;
	SMStreamId: string;
}
export interface SetSMStreamRanksRequest
{
	Requests: SMChannelStreamRankRequest[];
}
export interface GetSMChannelChannelsRequest
{
	SMChannelId: number;
}
export interface AddSMChannelToSMChannelRequest
{
	ChildSMChannelId: number;
	ParentSMChannelId: number;
	Rank?: number;
}
export interface RemoveSMChannelFromSMChannelRequest
{
	ChildSMChannelId: number;
	ParentSMChannelId: number;
}
export interface SetSMChannelRanksRequest
{
	Requests: SMChannelChannelRankRequest[];
}
export interface GetSettingsRequest
{
}
export interface UpdateSettingParameters
{
	AdminPassword?: string;
	AdminUserName?: string;
	AppendChannelName?: boolean;
	AuthenticationMethod?: string;
	AutoSetEPG?: boolean;
	BackupEnabled?: boolean;
	BackupInterval?: number;
	BackupVersionsToKeep?: number;
	CleanURLs?: boolean;
	ClientReadTimeOutSeconds?: number;
	ClientUserAgent?: string;
	DefaultCommandProfileName?: string;
	DefaultCompression?: string;
	DefaultOutputProfileName?: string;
	DeviceID?: string;
	EnableSSL?: boolean;
	FFMPegExecutable?: string;
	FFProbeExecutable?: string;
	GlobalStreamLimit?: number;
	IconCacheExpirationDays?: number;
	LogoCache?: boolean;
	M3U8OutPutProfile?: string;
	MaxConnectRetry?: number;
	MaxConnectRetryTimeMS?: number;
	MaxLogFiles?: number;
	MaxLogFileSizeMB?: number;
	NameRegex?: string[];
	PrettyEPG?: boolean;
	SDSettings?: SDSettingsRequest;
	ShowClientHostNames?: boolean;
	ShowIntros?: string;
	ShowMessageVideos?: boolean;
	ShutDownDelay?: number;
	SSLCertPassword?: string;
	SSLCertPath?: string;
	STRMBaseURL?: string;
}
export interface UpdateSettingRequest
{
	Parameters: UpdateSettingParameters;
}
export interface GetAvailableCountriesRequest
{
}
export interface GetHeadendsByCountryPostalRequest
{
	Country: string;
	PostalCode: string;
}
export interface GetHeadendsToViewRequest
{
}
export interface GetLineupPreviewChannelRequest
{
	Lineup: string;
}
export interface GetSelectedStationIdsRequest
{
}
export interface GetStationChannelNamesRequest
{
}
export interface GetStationPreviewsRequest
{
}
export interface GetSubScribedHeadendsRequest
{
}
export interface GetSubscribedLineupsRequest
{
}
export interface AddHeadendToViewRequest
{
	Country: string;
	HeadendId: string;
	Postal: string;
}
export interface AddLineupRequest
{
	Lineup: string;
}
export interface StationRequest
{
	Lineup: string;
	StationId: string;
}
export interface AddStationRequest
{
	Requests: StationRequest[];
}
export interface RemoveHeadendToViewRequest
{
	Country: string;
	HeadendId: string;
	Postal: string;
}
export interface RemoveLineupRequest
{
	Lineup: string;
}
export interface RemoveStationRequest
{
	Requests: StationRequest[];
}
export interface SetStationsRequest
{
	Requests: StationRequest[];
}
export interface GetCommandProfilesRequest
{
}
export interface GetOutputProfileRequest
{
	OutputProfileName: string;
}
export interface GetOutputProfilesRequest
{
}
export interface AddCommandProfileRequest
{
	Command: string;
	Parameters: string;
	ProfileName: string;
}
export interface AddOutputProfileRequest
{
	OutputProfileDto: OutputProfileDto;
}
export interface RemoveCommandProfileRequest
{
	ProfileName: string;
}
export interface RemoveOutputProfileRequest
{
	Name: string;
}
export interface UpdateCommandProfileRequest
{
	Command?: string;
	NewProfileName?: string;
	Parameters?: string;
	ProfileName: string;
}
export interface UpdateOutputProfileRequest
{
	EnableChannelNumber?: boolean;
	EnableGroupTitle?: boolean;
	EnableIcon?: boolean;
	Group?: string;
	Id?: string;
	Name?: string;
	NewName?: string;
	ProfileName: string;
}
export interface GetM3UFileNamesRequest
{
}
export interface GetM3UFilesRequest
{
}
export interface GetPagedM3UFilesRequest
{
	Parameters: QueryStringParameters;
}
export interface CreateM3UFileFromFormRequest
{
	AutoSetChannelNumbers?: boolean;
	DefaultStreamGroupName?: string;
	FormFile?: any;
	HoursToUpdate?: number;
	M3U8OutPutProfile?: string;
	M3UKey?: M3UKey;
	M3UName?: M3UField;
	MaxStreamCount?: number;
	Name: string;
	StartingChannelNumber?: number;
	SyncChannels?: boolean;
	VODTags?: string[];
}
export interface CreateM3UFileRequest
{
	AutoSetChannelNumbers?: boolean;
	DefaultStreamGroupName?: string;
	HoursToUpdate?: number;
	M3U8OutPutProfile?: string;
	M3UKey?: M3UKey;
	M3UName?: M3UField;
	MaxStreamCount?: number;
	Name: string;
	StartingChannelNumber?: number;
	SyncChannels?: boolean;
	UrlSource?: string;
	VODTags?: string[];
}
export interface DeleteM3UFileRequest
{
	DeleteFile: boolean;
	Id: number;
}
export interface ProcessM3UFileRequest
{
	ForceRun: boolean;
	M3UFileId: number;
}
export interface RefreshM3UFileRequest
{
	ForceRun: boolean;
	Id: number;
}
export interface SyncChannelsRequest
{
	Group?: string;
	M3UFileId: number;
}
export interface UpdateM3UFileRequest
{
	AutoSetChannelNumbers?: boolean;
	AutoUpdate?: boolean;
	DefaultStreamGroupName?: string;
	HoursToUpdate?: number;
	Id: number;
	M3U8OutPutProfile?: string;
	M3UName?: M3UField;
	MaxStreamCount?: number;
	Name?: string;
	StartingChannelNumber?: number;
	SyncChannels?: boolean;
	Url?: string;
	VODTags?: string[];
}
export interface GetLogContentsRequest
{
	LogName: string;
}
export interface GetLogNamesRequest
{
}
export interface GetCustomLogosRequest
{
}
export interface GetLogoForChannelRequest
{
	SMChannelId: number;
}
export interface GetLogoRequest
{
	Url: string;
}
export interface GetLogosRequest
{
}
export interface AddCustomLogoRequest
{
	Name: string;
	Source: string;
}
export interface RemoveCustomLogoRequest
{
	Source: string;
}
export interface GetDownloadServiceStatusRequest
{
}
export interface GetIsSystemReadyRequest
{
}
export interface GetSystemStatusRequest
{
}
export interface GetTaskIsRunningRequest
{
}
export interface SetTestTaskRequest
{
	DelayInSeconds: number;
}
export interface GetEPGColorsRequest
{
}
export interface EPGSyncRequest
{
}
export interface GetEPGFileNamesRequest
{
}
export interface GetEPGFilePreviewByIdRequest
{
	Id: number;
}
export interface GetEPGFilesRequest
{
}
export interface GetEPGNextEPGNumberRequest
{
}
export interface GetPagedEPGFilesRequest
{
	Parameters: QueryStringParameters;
}
export interface CreateEPGFileFromFormRequest
{
	Color?: string;
	EPGNumber: number;
	FormFile?: any;
	HoursToUpdate?: number;
	Name: string;
	TimeShift?: number;
}
export interface CreateEPGFileRequest
{
	Color?: string;
	EPGNumber: number;
	FileName: string;
	HoursToUpdate?: number;
	Name: string;
	TimeShift?: number;
	UrlSource?: string;
}
export interface DeleteEPGFileRequest
{
	DeleteFile: boolean;
	Id: number;
}
export interface ProcessEPGFileRequest
{
	Id: number;
}
export interface RefreshEPGFileRequest
{
	Id: number;
}
export interface UpdateEPGFileRequest
{
	AutoUpdate?: boolean;
	Color?: string;
	EPGNumber?: number;
	HoursToUpdate?: number;
	Id: number;
	Name?: string;
	TimeShift?: number;
	Url?: string;
}
export interface GetCustomPlayListRequest
{
	SMStreamId?: string;
}
export interface GetCustomPlayListsRequest
{
}
export interface GetIntroPlayListsRequest
{
}
export interface ScanForCustomRequest
{
}
export interface GetChannelGroupsFromSMChannelsRequest
{
}
export interface GetChannelGroupsRequest
{
}
export interface GetPagedChannelGroupsRequest
{
	Parameters: QueryStringParameters;
}
export interface CreateChannelGroupRequest
{
	GroupName: string;
	IsReadOnly: boolean;
}
export interface DeleteAllChannelGroupsFromParametersRequest
{
	Parameters: QueryStringParameters;
}
export interface DeleteChannelGroupRequest
{
	ChannelGroupId: number;
}
export interface DeleteChannelGroupsRequest
{
	ChannelGroupIds: number[];
}
export interface UpdateChannelGroupRequest
{
	ChannelGroupId: number;
	IsHidden?: boolean;
	NewGroupName?: string;
	ToggleVisibility?: boolean;
}
export interface UpdateChannelGroupsRequest
{
	UpdateChannelGroupRequests: UpdateChannelGroupRequest[];
}
export interface BaseStatistics
{
	ElapsedTime: string;
	StartTime: any;
}
export interface BPSStatistics
{
	BitsPerSecond: number;
	BytesRead: number;
	BytesWritten: number;
	Clients: number;
	ElapsedTime: string;
	IsSet: boolean;
	StartTime: any;
}
export interface ChannelStreamingStatistics
{
	BitsPerSecond: number;
	BytesRead: number;
	BytesWritten: number;
	ChannelLogo?: string;
	ChannelName: string;
	ChannelUrl: string;
	Clients: number;
	CurrentRank: number;
	CurrentStreamId: string;
	ElapsedTime: string;
	Id: number;
	IsSet: boolean;
	StartTime: any;
	StreamStreamingStatistics: StreamStreamingStatistic[];
}
export interface ClientStatistics
{
	Clients: number;
	ElapsedTime: string;
	StartTime: any;
}
export interface ClientStreamingStatistics
{
	BitsPerSecond: number;
	BytesRead: number;
	BytesWritten: number;
	ChannelId: number;
	ChannelName: string;
	ClientAgent: string;
	ClientIPAddress: string;
	Clients: number;
	ElapsedTime: string;
	IsSet: boolean;
	StartTime: any;
	UniqueRequestId: string;
}
export interface StreamHandlerMetrics
{
	AverageLatency: number;
	BytesRead: number;
	BytesWritten: number;
	ErrorCount: number;
	Kbps: number;
	StartTime: any;
}
export interface StreamStreamingStatistic
{
	BitsPerSecond: number;
	BytesRead: number;
	BytesWritten: number;
	Clients: number;
	ElapsedTime: string;
	Id: string;
	IsSet: boolean;
	Rank: number;
	StartTime: any;
	StreamLogo?: string;
	StreamName: string;
	StreamUrl?: string;
}
export interface ChannelMetric
{
	ChannelItemBackLog: number;
	ClientChannels: ClientChannelDto[];
	ClientStreams: ClientStreamsDto[];
	Id: string;
	IsFailed: boolean;
	Logo?: string;
	Metrics: StreamHandlerMetrics;
	Name: string;
	SMStreamInfo?: SMStreamInfo;
	SourceName: string;
	TotalBytesInBuffer: number;
	VideoInfo?: string;
}
export interface ClientChannelDto
{
	ClientIPAddress?: string;
	ClientUserAgent?: string;
	Logo?: string;
	Metrics?: StreamHandlerMetrics;
	Name: string;
	SMChannelId: number;
}
export interface ClientStreamsDto
{
	ClientIPAddress?: string;
	ClientUserAgent?: string;
	Logo?: string;
	Name: string;
	SMStreamId: string;
}
export interface CustomStreamNfo
{
	Movie: Movie;
	VideoFileName: string;
}
export interface Actor
{
	Name: string;
	Order?: string;
	Role?: string;
	Thumb?: string;
}
export interface Audio
{
	Bitrate?: string;
	Channels?: string;
	Codec?: string;
	Language?: string;
}
export interface CustomPlayList
{
	CustomStreamNfos: CustomStreamNfo[];
	FolderNfo?: Movie;
	Logo: string;
	Name: string;
}
export interface Fanart
{
	Thumb?: Thumb;
}
export interface Fileinfo
{
	Streamdetails?: Streamdetails;
}
export interface Movie
{
	Actors?: Actor[];
	Artworks?: string[];
	Country?: string;
	Credits?: string[];
	Criticrating?: string;
	Directors?: string[];
	Fanart?: Fanart;
	Fileinfo?: Fileinfo;
	Genres?: string[];
	Id?: string;
	Lastplayed?: string;
	Mpaa?: string;
	Originaltitle?: string;
	Outline?: string;
	Playcount?: string;
	Plot?: string;
	Premiered: string;
	Rating?: string;
	Ratings?: Ratings;
	Runtime: number;
	Set?: Set;
	Sorttitle?: string;
	Status?: string;
	Studio?: string;
	Tagline?: string;
	Thumb?: Thumb;
	Title: string;
	Top250?: string;
	Trailers?: string[];
	Uniqueids?: Uniqueid[];
	Userrating?: string;
	Watched?: string;
	Year?: string;
}
export interface Rating
{
	Default?: string;
	Max?: string;
	Name?: string;
	Value: string;
	Votes?: string;
}
export interface Ratings
{
	Rating?: Rating[];
}
export interface Set
{
	Name?: string;
	Overview?: string;
}
export interface Streamdetails
{
	Audio?: Audio;
	Subtitle?: Subtitle;
	Video?: Video;
}
export interface Subtitle
{
	Language?: string;
}
export interface Thumb
{
	Aspect?: string;
	Preview?: string;
	Text?: string;
}
export interface Uniqueid
{
	Default?: string;
	Text?: string;
	Type?: string;
}
export interface Video
{
	Aspect?: string;
	Bitrate?: string;
	Codec?: string;
	Duration?: string;
	Durationinseconds?: string;
	Framerate?: string;
	Height?: string;
	Scantype?: string;
	Width?: string;
}
export enum AuthenticationType {
	None = 0,
	Forms = 2
}
export enum JobType {
	ProcessM3U = 0,
	RefreshM3U = 1,
	TimerM3U = 2,
	ProcessEPG = 3,
	RefreshEPG = 4,
	UpdateEPG = 5,
	UpdateM3U = 6,
	TimerEPG = 7,
	SDSync = 8,
	Backup = 9,
	TimerBackup = 10,
	EPGRemovedExpiredKeys = 11
}
export enum M3UField {
	ChannelId = 0,
	ChannelName = 1,
	ChannelNumber = 2,
	Group = 3,
	Name = 4,
	TvgID = 5,
	TvgName = 6
}
export enum M3UFileStreamURLPrefix {
	SystemDefault = 0,
	TS = 1,
	M3U8 = 2
}
export enum M3UKey {
	URL = 0,
	CUID = 1,
	ChannelId = 2,
	TvgID = 3,
	TvgName = 4,
	TvgName_TvgID = 5,
	Name = 6,
	Name_TvgID = 7
}
export enum SMChannelTypeEnum {
	Regular = 0,
	MultiView = 1
}
export enum SMStreamTypeEnum {
	Regular = 0,
	CustomPlayList = 1,
	Custom = 2,
	Intro = 3,
	Message = 4
}
export enum SMFileTypes {
	M3U = 0,
	EPG = 1,
	HDHR = 2,
	Channel = 3,
	M3UStream = 4,
	Image = 5,
	TvLogo = 6,
	Logo = 7,
	CustomLogo = 8,
	CustomPlayList = 9,
	CustomPlayListLogo = 10,
	ProgramLogo = 11
}
export enum ValidM3USetting {
	NotMapped = 0,
	Name = 2,
	Group = 3,
	ChannelNumber = 4,
	ChannelName = 5
}
export enum VideoStreamHandlers {
	SystemDefault = 0,
	None = 1,
	Loop = 2
}
