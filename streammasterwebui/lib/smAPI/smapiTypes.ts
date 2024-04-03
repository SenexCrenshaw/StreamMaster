export interface PagedResponse<T>
{
	data: T[];
	first: number;
	pageNumber: number;
	pageSize: number;
	totalItemCount: number;
	totalPageCount: number;
}
export interface QueryStringParameters
{
	jsonArgumentString?: string;
	jsonFiltersString?: string;
	orderBy: string;
	pageNumber: number;
	pageSize: number;
}
export interface SMChannelRankRequest
{
	rank: number;
	smChannelId: number;
	smStreamId: string;
}
export interface FieldData
{
	entity: string;
	field: string;
	id: string;
	value: any;
}
export interface SMMessage
{
	detail?: string;
	severity: string;
	summary: string;
}
export interface StreamGroupSMChannel
{
	isReadOnly: boolean;
	rank: number;
	smChannel: any;
	smChannelId: number;
	streamGroupId: number;
}
export interface ChannelGroupDto
{
	activeCount: number;
	channelGroupId: number;
	hiddenCount: number;
	id: number;
	isHidden: boolean;
	isReadOnly: boolean;
	name: string;
	totalCount: number;
}
export interface EPGFileDto
{
	autoUpdate: boolean;
	channelCount: number;
	color: string;
	description: string;
	downloadErrors: number;
	epgNumber: number;
	epgStartDate: any;
	epgStopDate: any;
	hoursToUpdate: number;
	id: number;
	lastDownloadAttempt: any;
	lastDownloaded: any;
	name: string;
	needsUpdate: boolean;
	programmeCount: number;
	source: string;
	timeShift: number;
	url: string;
}
export interface IconFileDto
{
	extension: string;
	fileId: number;
	id: string;
	name: string;
	smFileType: number;
	source: string;
}
export interface M3UFileDto
{
	autoUpdate: boolean;
	description: string;
	downloadErrors: number;
	hoursToUpdate: number;
	id: number;
	lastDownloadAttempt: any;
	lastDownloaded: any;
	maxStreamCount: number;
	name: string;
	needsUpdate: boolean;
	overwriteChannelNumbers: boolean;
	source: string;
	startingChannelNumber: number;
	stationCount: number;
	url: string;
	vodTags: string[];
}
export interface SettingDto
{
	adminPassword: string;
	adminUserName: string;
	apiKey: string;
	authenticationMethod: number;
	backupEnabled: boolean;
	backupInterval: number;
	backupVersionsToKeep: number;
	cacheIcons: boolean;
	cleanURLs: boolean;
	clientUserAgent: string;
	defaultIcon: string;
	deviceID: string;
	dummyRegex: string;
	enablePrometheus: boolean;
	enableSSL: boolean;
	expectedServiceCount: number;
	ffmpegDefaultOptions: string;
	ffmPegExecutable: string;
	ffMpegOptions: string;
	ffProbeExecutable: string;
	globalStreamLimit: number;
	hls: HLSSettings;
	isDebug: boolean;
	logPerformance: string[];
	m3UFieldGroupTitle: boolean;
	m3UIgnoreEmptyEPGID: boolean;
	m3UStationId: boolean;
	m3UUseChnoForId: boolean;
	m3UUseCUIDForChannelID: boolean;
	maxConcurrentDownloads: number;
	maxConnectRetry: number;
	maxConnectRetryTimeMS: number;
	maxLogFiles: number;
	maxLogFileSizeMB: number;
	maxStreamReStart: number;
	nameRegex: string[];
	prettyEPG: boolean;
	release: string;
	sdSettings: SDSettings;
	showClientHostNames: boolean;
	sslCertPassword: string;
	sslCertPath: string;
	streamingClientUserAgent: string;
	streamingProxyType: number;
	uiFolder: string;
	urlBase: string;
	version: string;
	videoStreamAlwaysUseEPGLogo: boolean;
}
export interface SMChannelDto
{
	channelNumber: number;
	epgId: string;
	group: string;
	groupTitle: string;
	id: number;
	isHidden: boolean;
	logo: string;
	name: string;
	realUrl: string;
	smStreams: SMStreamDto[];
	stationId: string;
	streamGroups: StreamGroupSMChannel[];
	streamingProxyType: number;
	timeShift: number;
	videoStreamHandler: number;
	videoStreamId: string;
}
export interface SMStreamDto
{
	channelNumber: number;
	epgid: string;
	filePosition: number;
	group: string;
	id: string;
	isHidden: boolean;
	isUserCreated: boolean;
	logo: string;
	m3UFileId: number;
	m3UFileName: string;
	name: string;
	rank: number;
	realUrl: string;
	shortId: string;
	stationId: string;
	url: string;
}
export interface StreamGroupDto
{
	autoSetChannelNumbers: boolean;
	ffmpegProfileId: string;
	hdhrLink: string;
	id: number;
	isLoading: boolean;
	isReadOnly: boolean;
	m3ULink: string;
	name: string;
	shortEPGLink: string;
	shortM3ULink: string;
	streamCount: number;
	xmlLink: string;
}
export interface HLSSettings
{
	hlsffmpegOptions: string;
	hlsM3U8CreationTimeOutInSeconds: number;
	hlsM3U8Enable: boolean;
	hlsM3U8ReadTimeOutInSeconds: number;
	hlsReconnectDurationInSeconds: number;
	hlsSegmentCount: number;
	hlsSegmentDurationInSeconds: number;
	hlstsReadTimeOutInSeconds: number;
}
export interface SDSettings
{
	alternateLogoStyle: string;
	alternateSEFormat: boolean;
	appendEpisodeDesc: boolean;
	artworkSize: string;
	excludeCastAndCrew: boolean;
	preferredLogoStyle: string;
	prefixEpisodeDescription: boolean;
	prefixEpisodeTitle: boolean;
	sdCountry: string;
	sdEnabled: boolean;
	sdepgDays: number;
	sdPassword: string;
	sdPostalCode: string;
	sdStationIds: any[];
	sdUserName: string;
	seasonEventImages: boolean;
	seriesPosterArt: boolean;
	seriesPosterAspect: string;
	seriesWsArt: boolean;
	xmltvAddFillerData: boolean;
	xmltvExtendedInfoInTitleDescriptions: boolean;
	xmltvFillerProgramLength: number;
	xmltvIncludeChannelNumbers: boolean;
	xmltvSingleImage: boolean;
}
export interface APIResponse<T>
{
	errorMessage?: string;
	isError?: boolean;
	message?: string;
	pagedResponse?: PagedResponse<T>;
}
export interface DefaultAPIResponse
{
	errorMessage?: string;
	isError?: boolean;
	message?: string;
	notFound: DefaultAPIResponse;
	ok: DefaultAPIResponse;
	pagedResponse?: PagedResponse<NoClass>;
}
export interface NoClass
{
}
export interface GetPagedStreamGroupsRequest
{
	parameters: QueryStringParameters;
}
export interface GetPagedSMStreamsRequest
{
	parameters: QueryStringParameters;
}
export interface ToggleSMStreamVisibleByIdRequest
{
	id: string;
}
export interface SendSMErrorRequest
{
	detail: string;
	summary: string;
}
export interface SendSMInfoRequest
{
	detail: string;
	summary: string;
}
export interface SendSMMessageRequest
{
	message: SMMessage;
}
export interface SendSMWarnRequest
{
	detail: string;
	summary: string;
}
export interface SendSuccessRequest
{
	detail: string;
	summary: string;
}
export interface AddSMStreamToSMChannelRequest
{
	smChannelId: number;
	smStreamId: string;
}
export interface CreateSMChannelFromStreamRequest
{
	streamId: string;
}
export interface DeleteSMChannelRequest
{
	smChannelId: number;
}
export interface DeleteSMChannelsFromParametersRequest
{
	parameters: QueryStringParameters;
}
export interface DeleteSMChannelsRequest
{
	smChannelIds: number[];
}
export interface GetPagedSMChannelsRequest
{
	parameters: QueryStringParameters;
}
export interface RemoveSMStreamFromSMChannelRequest
{
	smChannelId: number;
	smStreamId: string;
}
export interface SetSMChannelLogoRequest
{
	logo: string;
	smChannelId: number;
}
export interface SetSMChannelNameRequest
{
	name: string;
	smChannelId: number;
}
export interface SetSMChannelNumberRequest
{
	channelNumber: number;
	smChannelId: number;
}
export interface SetSMStreamRanksRequest
{
	requests: SMChannelRankRequest[];
}
export interface SDSystemStatus
{
	isSystemReady: boolean;
}
export interface GetIsSystemReadyRequest
{
}
export interface GetSettingsRequest
{
}
export interface GetSystemStatusRequest
{
}
export interface CreateM3UFileRequest
{
	formFile?: any;
	maxStreamCount: number;
	name: string;
	overWriteChannels?: boolean;
	startingChannelNumber?: number;
	urlSource?: string;
	vodTags?: string[];
}
export interface DeleteM3UFileRequest
{
	deleteFile: boolean;
	id: number;
}
export interface GetPagedM3UFilesRequest
{
	parameters: QueryStringParameters;
}
export interface ProcessM3UFileRequest
{
	forceRun: boolean;
	m3UFileId: number;
}
export interface ProcessM3UFilesRequest
{
}
export interface RefreshM3UFileRequest
{
	forceRun: boolean;
	id: number;
}
export interface UpdateM3UFileRequest
{
	autoUpdate?: boolean;
	hoursToUpdate?: number;
	id: number;
	maxStreamCount?: number;
	name?: string;
	overWriteChannels?: boolean;
	startingChannelNumber?: number;
	url?: string;
	vodTags?: string[];
}
export interface GetIconsRequest
{
}
export interface CreateChannelGroupRequest
{
	groupName: string;
	isReadOnly: boolean;
}
export interface GetPagedChannelGroupsRequest
{
	parameters: QueryStringParameters;
}
