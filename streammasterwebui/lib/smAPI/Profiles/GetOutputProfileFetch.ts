import { GetOutputProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { GetOutputProfileRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetOutputProfile = createAsyncThunk('cache/getGetOutputProfile', async (param: GetOutputProfileRequest, thunkAPI) => {
  try {
    Logger.debug('Fetching GetOutputProfile');
    const response = await GetOutputProfile(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


