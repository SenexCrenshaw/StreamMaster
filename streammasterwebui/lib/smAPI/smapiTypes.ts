export interface PagedResponse<T>
{
	data: T[];
	pageNumber: number;
	pageSize: number;
	totalPageCount: number;
	totalItemCount: number;
	first: number;
}
export interface QueryStringParameters
{
	pageNumber: number;
	pageSize: number;
	orderBy: string;
	jsonArgumentString?: string;
	jsonFiltersString?: string;
}
export interface SMChannelRankRequest
{
	smChannelId: number;
	smStreamId: string;
	rank: number;
}
export interface FieldData
{
	entity: string;
	id: string;
	field: string;
	value: any;
}
export interface SMMessage
{
	severity: string;
	summary: string;
	detail?: string;
}
export interface StreamGroupSMChannel
{
	smChannel: any;
	smChannelId: number;
	isReadOnly: boolean;
	streamGroupId: number;
	rank: number;
}
export interface M3UFileDto
{
	vodTags: string[];
	overwriteChannelNumbers: boolean;
	startingChannelNumber: number;
	maxStreamCount: number;
	stationCount: number;
	source: string;
	autoUpdate: boolean;
	description: string;
	downloadErrors: number;
	hoursToUpdate: number;
	id: number;
	lastDownloadAttempt: any;
	lastDownloaded: any;
	name: string;
	needsUpdate: boolean;
	url: string;
}
export interface SettingDto
{
	sdSettings: SDSettings;
	hls: HLSSettings;
	release: string;
	version: string;
	ffmpegDefaultOptions: string;
	isDebug: boolean;
	m3UFieldGroupTitle: boolean;
	m3UIgnoreEmptyEPGID: boolean;
	m3UUseChnoForId: boolean;
	m3UUseCUIDForChannelID: boolean;
	m3UStationId: boolean;
	backupEnabled: boolean;
	backupVersionsToKeep: number;
	backupInterval: number;
	prettyEPG: boolean;
	maxLogFiles: number;
	maxLogFileSizeMB: number;
	enablePrometheus: boolean;
	maxStreamReStart: number;
	maxConcurrentDownloads: number;
	expectedServiceCount: number;
	adminPassword: string;
	adminUserName: string;
	defaultIcon: string;
	uiFolder: string;
	urlBase: string;
	logPerformance: string[];
	apiKey: string;
	authenticationMethod: number;
	cacheIcons: boolean;
	cleanURLs: boolean;
	clientUserAgent: string;
	deviceID: string;
	dummyRegex: string;
	ffMpegOptions: string;
	enableSSL: boolean;
	ffmPegExecutable: string;
	ffProbeExecutable: string;
	globalStreamLimit: number;
	maxConnectRetry: number;
	maxConnectRetryTimeMS: number;
	nameRegex: string[];
	sslCertPassword: string;
	sslCertPath: string;
	streamingClientUserAgent: string;
	streamingProxyType: number;
	videoStreamAlwaysUseEPGLogo: boolean;
	showClientHostNames: boolean;
}
export interface SMChannelDto
{
	smStreams: SMStreamDto[];
	realUrl: string;
	streamingProxyType: number;
	streamGroups: StreamGroupSMChannel[];
	id: number;
	isHidden: boolean;
	channelNumber: number;
	timeShift: number;
	group: string;
	epgId: string;
	logo: string;
	name: string;
	stationId: string;
	groupTitle: string;
	videoStreamHandler: number;
	videoStreamId: string;
}
export interface SMStreamDto
{
	rank: number;
	realUrl: string;
	id: string;
	filePosition: number;
	isHidden: boolean;
	isUserCreated: boolean;
	m3UFileId: number;
	channelNumber: number;
	m3UFileName: string;
	shortId: string;
	group: string;
	epgid: string;
	logo: string;
	name: string;
	url: string;
	stationId: string;
}
export interface HLSSettings
{
	hlsM3U8Enable: boolean;
	hlsffmpegOptions: string;
	hlsReconnectDurationInSeconds: number;
	hlsSegmentDurationInSeconds: number;
	hlsSegmentCount: number;
	hlsM3U8CreationTimeOutInSeconds: number;
	hlsM3U8ReadTimeOutInSeconds: number;
	hlstsReadTimeOutInSeconds: number;
}
export interface SDSettings
{
	alternateSEFormat: boolean;
	alternateLogoStyle: string;
	appendEpisodeDesc: boolean;
	artworkSize: string;
	excludeCastAndCrew: boolean;
	preferredLogoStyle: string;
	prefixEpisodeDescription: boolean;
	prefixEpisodeTitle: boolean;
	sdEnabled: boolean;
	sdepgDays: number;
	sdCountry: string;
	sdPassword: string;
	sdPostalCode: string;
	sdStationIds: any[];
	sdUserName: string;
	seasonEventImages: boolean;
	seriesPosterArt: boolean;
	seriesPosterAspect: string;
	seriesWsArt: boolean;
	xmltvAddFillerData: boolean;
	xmltvFillerProgramLength: number;
	xmltvExtendedInfoInTitleDescriptions: boolean;
	xmltvIncludeChannelNumbers: boolean;
	xmltvSingleImage: boolean;
}
export interface APIResponse<T>
{
	message?: string;
	errorMessage?: string;
	isError?: boolean;
	pagedResponse?: PagedResponse<T>;
}
export interface DefaultAPIResponse
{
	message?: string;
	errorMessage?: string;
	isError?: boolean;
	pagedResponse?: PagedResponse<NoClass>;
}
export interface NoClass
{
}
export interface SDSystemStatus
{
	isSystemReady: boolean;
}
export interface CreateM3UFileRequest
{
	name: string;
	maxStreamCount: number;
	urlSource?: string;
	overWriteChannels?: boolean;
	startingChannelNumber?: number;
	formFile?: any;
	vodTags?: string[];
}
export interface DeleteM3UFileRequest
{
	deleteFile: boolean;
	id: number;
}
export interface ProcessM3UFileRequest
{
	m3UFileId: number;
	forceRun: boolean;
}
export interface ProcessM3UFilesRequest
{
}
export interface RefreshM3UFileRequest
{
	id: number;
	forceRun: boolean;
}
export interface CreateChannelGroupRequest
{
	groupName: string;
	isReadOnly: boolean;
}
