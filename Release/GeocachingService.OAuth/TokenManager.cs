/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;

namespace GeocachingService.OAuth
{
    public class TokenManager : DotNetOpenAuth.OAuth.ChannelElements.IConsumerTokenManager
    {
        private class Token
        {
            public DotNetOpenAuth.OAuth.ChannelElements.TokenType TokenType;
            public string TokenKey;
            public string Secret;
        }

        private System.Collections.Concurrent.ConcurrentDictionary<string, Token> _tokenCache = new System.Collections.Concurrent.ConcurrentDictionary<string, Token>(StringComparer.Ordinal);

        public string ConsumerKey
        {
            get;
            private set;
        }

        public string ConsumerSecret
        {
            get;
            private set;
        }

        public TokenManager(string consumerKey, string consumerSecret)
        {
            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
        }

        public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret)
        {
            Token temp;
            this._tokenCache.TryRemove(requestToken, out temp);
            var newToken = new Token() { TokenType = DotNetOpenAuth.OAuth.ChannelElements.TokenType.AccessToken, TokenKey = accessToken, Secret = accessTokenSecret };
            this._tokenCache.AddOrUpdate(accessToken, newToken, (t, old) => newToken);
        }

        public string GetTokenSecret(string token)
        {
            Token val;
            if (!this._tokenCache.TryGetValue(token, out val))
                return null;

            return val.Secret;
        }

        public DotNetOpenAuth.OAuth.ChannelElements.TokenType GetTokenType(string token)
        {
            Token val;
            if (!this._tokenCache.TryGetValue(token, out val))
                return DotNetOpenAuth.OAuth.ChannelElements.TokenType.InvalidToken;

            return val.TokenType;
        }

        public void StoreNewRequestToken(DotNetOpenAuth.OAuth.Messages.UnauthorizedTokenRequest request, DotNetOpenAuth.OAuth.Messages.ITokenSecretContainingMessage response)
        {
            var newToken = new Token() { TokenType = DotNetOpenAuth.OAuth.ChannelElements.TokenType.RequestToken, TokenKey = response.Token, Secret = response.TokenSecret};
            this._tokenCache.AddOrUpdate(newToken.TokenKey, newToken, (t, old) => newToken);
        }
    }
}