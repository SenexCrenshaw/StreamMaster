export interface QueryStringParameters
{
	JSONArgumentString?: string;
	JSONFiltersString?: string;
	OrderBy: string;
	PageNumber: number;
	PageSize: number;
}
export interface SMChannelRankRequest
{
	Rank: number;
	SMChannelId: number;
	SMStreamId: string;
}
export interface FieldData
{
	Entity: string;
	Field: string;
	Id: string;
	Value: any;
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
	ShutDownDelay: number;
	SMStreamType: number;
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
	ChannelNumber: string;
	Id: string;
}
export interface IconFileDto
{
	Extension: string;
	FileId: number;
	Id: string;
	Name: string;
	SMFileType: SMFileTypes;
	Source: string;
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
	AuthenticationMethod: string;
	AutoSetEPG: boolean;
	BackupEnabled: boolean;
	BackupInterval: number;
	BackupVersionsToKeep: number;
	CacheIcons: boolean;
	CleanURLs: boolean;
	ClientUserAgent: string;
	DefaultCommandProfileName: string;
	DefaultIcon: string;
	DefaultOutputProfileName: string;
	DeviceID: string;
	DummyRegex: string;
	EnableSSL: boolean;
	FFMPegExecutable: string;
	FFProbeExecutable: string;
	GlobalStreamLimit: number;
	IsDebug: boolean;
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
	SSLCertPassword: string;
	SSLCertPath: string;
	UiFolder: string;
	UrlBase: string;
	Version: string;
	VideoStreamAlwaysUseEPGLogo: boolean;
}
export interface SMChannelDto
{
	APIName: string;
	BaseStreamID: string;
	ChannelNumber: number;
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
	SMChannels: any[];
	SMChannelType: number;
	SMStreams: SMStreamDto[];
	StationId: string;
	StreamGroupIds: number[];
	StreamGroups: StreamGroupSMChannelLink[];
	StreamUrl: string;
	TimeShift: number;
}
export interface SMStreamDto
{
	APIName: string;
	ChannelNumber: number;
	ClientUserAgent?: string;
	CUID: string;
	EPGID: string;
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
	Rank: number;
	RealUrl: string;
	SMStreamType: number;
	StationId: string;
	Url: string;
}
export interface StreamGroupDto
{
	AutoSetChannelNumbers: boolean;
	ChannelCount: number;
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
	EnableId: boolean;
	EPGId: string;
	Group: string;
	IsReadOnly: boolean;
	Name: string;
}
export interface OutputProfileDto
{
	APIName: string;
	EnableChannelNumber: boolean;
	EnableGroupTitle: boolean;
	EnableIcon: boolean;
	EnableId: boolean;
	EPGId: string;
	Group: string;
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
	ExcludeCastAndCrew: boolean;
	HeadendsToView: any[];
	MaxSubscribedLineups: number;
	PreferredLogoStyle: string;
	PrefixEpisodeDescription: boolean;
	PrefixEpisodeTitle: boolean;
	SDCountry: string;
	SDEnabled: boolean;
	SDEPGDays: number;
	SDPassword: string;
	SDPostalCode: string;
	SDStationIds: StationIdLineup[];
	SDUserName: string;
	SeasonEventImages: boolean;
	SeriesPosterArt: boolean;
	SeriesPosterAspect: string;
	SeriesWsArt: boolean;
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
	SeasonEventImages?: boolean;
	SeriesPosterArt?: boolean;
	SeriesPosterAspect?: string;
	SeriesWsArt?: boolean;
	XmltvAddFillerData?: boolean;
	XmltvExtendedInfoInTitleDescriptions?: boolean;
	XmltvFillerProgramLength?: number;
	XmltvIncludeChannelNumbers?: boolean;
	XmltvSingleImage?: boolean;
}
export interface UpdateSettingResponse
{
	NeedsLogOut: boolean;
	Settings: SettingDto;
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
	_totalItemCount?: number;
	Count: number;
	Data: T;
	ErrorMessage?: string;
	IsError: boolean;
	Message?: string;
	NotFound: DataResponse<T>;
	Ok: DataResponse<T>;
	TotalItemCount: number;
}
export interface NoClass
{
}
export interface PagedResponse<T>
{
	_totalItemCount?: number;
	Count: number;
	Data: T[];
	ErrorMessage?: string;
	First: number;
	IsError: boolean;
	Message?: string;
	NotFound: DataResponse<T[]>;
	Ok: DataResponse<T[]>;
	PageNumber: number;
	PageSize: number;
	TotalItemCount: number;
	TotalPageCount: number;
}
export interface CountryData
{
	Countries: Country[];
	Id: string;
	Key: string;
}
export interface Logo
{
	Height: number;
	Md5: string;
	URL: string;
	Width: number;
}
export interface StationChannelName
{
	Channel: string;
	ChannelName: string;
	DisplayName: string;
	Id: string;
}
export interface StationPreview
{
	Affiliate: string;
	Callsign: string;
	Country: string;
	Id: string;
	Lineup: string;
	Logo: Logo;
	Name: string;
	PostalCode: string;
	StationId: string;
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
export interface LineupPreviewChannel
{
	Affiliate: string;
	Callsign: string;
	Channel: string;
	Id: number;
	Name: string;
}
export interface StationChannelMap
{
	Id: any;
	Map: LineupChannelStation[];
	Metadata?: LineupMetadata;
	Stations: LineupStation[];
}
export interface LineupChannelStation
{
	Channel: string;
	StationId: string;
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
	FrequencyHz?: number;
	LogicalChannelNumber: string;
	MatchName: string;
	MatchType: string;
	ModulationSystem: string;
	myChannelNumber: number;
	myChannelSubnumber: number;
	NetworkId?: number;
	Polarization: string;
	ProviderCallsign: string;
	ProviderChannel: string;
	ServiceId?: number;
	StationId: string;
	Symbolrate?: number;
	TransportId?: number;
	UhfVhf?: number;
	VirtualChannel: string;
}
export interface LineupStation
{
	Affiliate: string;
	Broadcaster: StationBroadcaster;
	BroadcastLanguage: string[];
	Callsign: string;
	DescriptionLanguage: string[];
	IsCommercialFree?: boolean;
	Logo: StationImage;
	Name: string;
	StationId: string;
	StationLogos: StationImage[];
}
export interface StationImage
{
	Category: string;
	Height: number;
	Md5: string;
	Source: string;
	Url: string;
	Width: number;
}
export interface StationBroadcaster
{
	City: string;
	Country: string;
	Postalcode: string;
	State: string;
}
export interface LineupMetadata
{
	Lineup: string;
	Modified: string;
	Modulation: string;
	Transport: string;
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
	Requests: SMChannelRankRequest[];
}
export interface GetSettingsRequest
{
}
export interface UpdateSettingParameters
{
	AdminPassword?: string;
	AdminUserName?: string;
	AuthenticationMethod?: string;
	BackupEnabled?: boolean;
	BackupInterval?: number;
	BackupVersionsToKeep?: number;
	CacheIcons?: boolean;
	CleanURLs?: boolean;
	ClientUserAgent?: string;
	DefaultCommandProfileName?: string;
	DefaultOutputProfileName?: string;
	DeviceID?: string;
	DummyRegex?: string;
	EnableSSL?: boolean;
	FFMPegExecutable?: string;
	FFProbeExecutable?: string;
	GlobalStreamLimit?: number;
	MaxConnectRetry?: number;
	MaxConnectRetryTimeMS?: number;
	MaxLogFiles?: number;
	MaxLogFileSizeMB?: number;
	NameRegex?: string[];
	PrettyEPG?: boolean;
	SDSettings?: SDSettingsRequest;
	ShowClientHostNames?: boolean;
	ShowIntros?: string;
	SSLCertPassword?: string;
	SSLCertPath?: string;
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
	EnableId?: boolean;
	EPGId?: string;
	Group?: string;
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
	DefaultStreamGroupName?: string;
	FormFile?: any;
	HoursToUpdate?: number;
	MaxStreamCount?: number;
	Name: string;
	SyncChannels?: boolean;
	VODTags?: string[];
}
export interface CreateM3UFileRequest
{
	DefaultStreamGroupName?: string;
	HoursToUpdate?: number;
	MaxStreamCount: number;
	Name: string;
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
	MaxStreamCount?: number;
	Name?: string;
	StartingChannelNumber?: number;
	SyncChannels?: boolean;
	Url?: string;
	VODTags?: string[];
}
export interface GetIconsRequest
{
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
	GetMetrics: StreamHandlerMetrics;
	Id: string;
	IsFailed: boolean;
	Logo?: string;
	Name: string;
	SMStreamInfo?: SMStreamInfo;
	SourceName: string;
}
export interface ClientChannelDto
{
	Logo?: string;
	Name: string;
	SMChannelId: number;
}
export interface ClientStreamsDto
{
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
	Order: string;
	Role: string;
	Thumb: string;
}
export interface Audio
{
	Bitrate: string;
	Channels: string;
	Codec: string;
	Language: string;
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
	Thumb: Thumb;
}
export interface Fileinfo
{
	Streamdetails: Streamdetails;
}
export interface Movie
{
	Actor: Actor[];
	Country: string;
	Criticrating: string;
	Fanart: Fanart;
	Fileinfo: Fileinfo;
	Genre: string[];
	Id: string;
	Lastplayed: string;
	Mpaa: string;
	Originaltitle: string;
	Outline: string;
	Playcount: string;
	Plot: string;
	Premiered: string;
	Rating: string;
	Ratings: Ratings;
	Runtime: number;
	Set: Set;
	Sorttitle: string;
	Status: string;
	Studio: string;
	Tagline: string;
	Thumb: Thumb;
	Title: string;
	Top250: string;
	Trailer: string;
	Uniqueid: Uniqueid[];
	Userrating: string;
	Watched: string;
	Year: string;
}
export interface Rating
{
	Default: string;
	Max: string;
	Name: string;
	Value: string;
	Votes: string;
}
export interface Ratings
{
	Rating: Rating[];
}
export interface Set
{
	Name: string;
	Overview: string;
}
export interface Streamdetails
{
	Audio: Audio;
	Subtitle: Subtitle;
	Video: Video;
}
export interface Subtitle
{
	Language: string;
}
export interface Thumb
{
	Aspect: string;
	Preview: string;
	Text: string;
}
export interface Uniqueid
{
	Default: string;
	Text: string;
	Type: string;
}
export interface Video
{
	Aspect: string;
	Bitrate: string;
	Codec: string;
	Duration: string;
	Durationinseconds: string;
	Framerate: string;
	Height: string;
	Scantype: string;
	Width: string;
}
export enum AuthenticationType {
	None = 0,
	Forms = 2
}
export enum M3UFileStreamURLPrefix {
	SystemDefault = 0,
	TS = 1,
	M3U8 = 2
}
export enum SMFileTypes {
	M3U = 0,
	EPG = 1,
	HDHR = 2,
	Channel = 3,
	M3UStream = 4,
	Icon = 5,
	Image = 6,
	TvLogo = 7,
	ProgrammeIcon = 8,
	ChannelIcon = 9,
	SDImage = 10,
	SDStationLogo = 11,
	CustomPlayList = 12,
	CustomPlayListArt = 13
}
export enum ValidM3USetting {
	NotMapped = 0,
	Name = 2,
	Group = 3,
	EPGId = 4,
	ChannelNumber = 5
}
export enum VideoStreamHandlers {
	SystemDefault = 0,
	None = 1,
	Loop = 2
}
