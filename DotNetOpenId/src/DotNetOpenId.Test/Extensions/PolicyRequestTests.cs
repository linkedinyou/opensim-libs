﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DotNetOpenId.Extensions.ProviderAuthenticationPolicy;
using DotNetOpenId.Extensions;
using System.Globalization;

namespace DotNetOpenId.Test.Extensions {
	[TestFixture]
	public class PolicyRequestTests {
		[Test]
		public void Ctor() {
			PolicyRequest req = new PolicyRequest();
			Assert.IsNull(req.MaximumAuthenticationAge);
			Assert.IsNotNull(req.PreferredPolicies);
			Assert.AreEqual(0, req.PreferredPolicies.Count);
		}

		[Test]
		public void MaximumAuthenticationAgeTest() {
			PolicyRequest req = new PolicyRequest();
			req.MaximumAuthenticationAge = TimeSpan.FromHours(1);
			Assert.IsNotNull(req.MaximumAuthenticationAge);
			Assert.AreEqual(TimeSpan.FromHours(1), req.MaximumAuthenticationAge);
			req.MaximumAuthenticationAge = null;
			Assert.IsNull(req.MaximumAuthenticationAge);
		}

		[Test]
		public void AddPolicies() {
			PolicyRequest resp = new PolicyRequest();
			resp.PreferredPolicies.Add(AuthenticationPolicies.MultiFactor);
			resp.PreferredPolicies.Add(AuthenticationPolicies.PhishingResistant);
			Assert.AreEqual(2, resp.PreferredPolicies.Count);
			Assert.AreEqual(AuthenticationPolicies.MultiFactor, resp.PreferredPolicies[0]);
			Assert.AreEqual(AuthenticationPolicies.PhishingResistant, resp.PreferredPolicies[1]);
		}

		[Test]
		public void AddPolicyMultipleTimes() {
			// Although this isn't really the desired behavior (we'd prefer to see an
			// exception thrown), since we're using a List<string> internally we can't
			// expect anything better (for now).  But if this is ever fixed, by all means
			// change this test to expect an exception or something else.
			PolicyRequest resp = new PolicyRequest();
			resp.PreferredPolicies.Add(AuthenticationPolicies.MultiFactor);
			resp.PreferredPolicies.Add(AuthenticationPolicies.MultiFactor);
			Assert.AreEqual(2, resp.PreferredPolicies.Count);
		}

		[Test]
		public void AddAuthLevelTypes() {
			PolicyRequest req = new PolicyRequest();
			req.PreferredAuthLevelTypes.Add(Constants.AuthenticationLevels.NistTypeUri);
			Assert.AreEqual(1, req.PreferredAuthLevelTypes.Count);
			Assert.IsTrue(req.PreferredAuthLevelTypes.Contains(Constants.AuthenticationLevels.NistTypeUri));
		}

		[Test]
		public void EqualsTest() {
			PolicyRequest req = new PolicyRequest();
			PolicyRequest req2 = new PolicyRequest();
			Assert.AreEqual(req, req2);
			Assert.AreNotEqual(req, null);
			Assert.AreNotEqual(null, req);

			// Test PreferredPolicies list comparison
			req.PreferredPolicies.Add(AuthenticationPolicies.PhishingResistant);
			Assert.AreNotEqual(req, req2);
			req2.PreferredPolicies.Add(AuthenticationPolicies.MultiFactor);
			Assert.AreNotEqual(req, req2);
			req2.PreferredPolicies.Clear();
			req2.PreferredPolicies.Add(AuthenticationPolicies.PhishingResistant);
			Assert.AreEqual(req, req2);

			// Test PreferredPolicies list comparison when that list is not in the same order.
			req.PreferredPolicies.Add(AuthenticationPolicies.MultiFactor);
			Assert.AreNotEqual(req, req2);
			req2.PreferredPolicies.Insert(0, AuthenticationPolicies.MultiFactor);
			Assert.AreEqual(req, req2);

			// Test MaximumAuthenticationAge comparison.
			req.MaximumAuthenticationAge = TimeSpan.FromHours(1);
			Assert.AreNotEqual(req, req2);
			req2.MaximumAuthenticationAge = req.MaximumAuthenticationAge;
			Assert.AreEqual(req, req2);

			// Test PreferredAuthLevelTypes comparison.
			req.PreferredAuthLevelTypes.Add("authlevel1");
			Assert.AreNotEqual(req, req2);
			req2.PreferredAuthLevelTypes.Add("authlevel2");
			Assert.AreNotEqual(req, req2);
			req.PreferredAuthLevelTypes.Add("authlevel2");
			req2.PreferredAuthLevelTypes.Add("authlevel1");
			Assert.AreEqual(req, req2);
		}

		[Test]
		public void DeserializeNull() {
			PolicyRequest req = new PolicyRequest();
			Assert.IsFalse(((IExtensionRequest)req).Deserialize(null, null, Constants.TypeUri));
		}

		[Test]
		public void DeserializeEmpty() {
			PolicyRequest req = new PolicyRequest();
			Assert.IsFalse(((IExtensionRequest)req).Deserialize(new Dictionary<string, string>(), null, Constants.TypeUri));
		}

		[Test]
		public void SerializeRoundTrip() {
			// This test relies on the PolicyRequest.Equals method.  If this and that test 
			// are failing, work on EqualsTest first.

			// Most basic test
			PolicyRequest req = new PolicyRequest(), req2 = new PolicyRequest();
			var fields = ((IExtensionRequest)req).Serialize(null);
			Assert.IsTrue(((IExtensionRequest)req2).Deserialize(fields, null, Constants.TypeUri));
			Assert.AreEqual(req, req2);

			// Test with all fields set
			req2 = new PolicyRequest();
			req.PreferredPolicies.Add(AuthenticationPolicies.MultiFactor);
			req.PreferredAuthLevelTypes.Add(Constants.AuthenticationLevels.NistTypeUri);
			req.MaximumAuthenticationAge = TimeSpan.FromHours(1);
			fields = ((IExtensionRequest)req).Serialize(null);
			Assert.IsTrue(((IExtensionRequest)req2).Deserialize(fields, null, Constants.TypeUri));
			Assert.AreEqual(req, req2);

			// Test with an extra policy and auth level
			req2 = new PolicyRequest();
			req.PreferredPolicies.Add(AuthenticationPolicies.PhishingResistant);
			req.PreferredAuthLevelTypes.Add("customAuthLevel");
			fields = ((IExtensionRequest)req).Serialize(null);
			Assert.IsTrue(((IExtensionRequest)req2).Deserialize(fields, null, Constants.TypeUri));
			Assert.AreEqual(req, req2);

			// Test with a policy added twice.  We should see it intelligently leave one of
			// the doubled policies out.
			req2 = new PolicyRequest();
			req.PreferredPolicies.Add(AuthenticationPolicies.PhishingResistant);
			req.PreferredAuthLevelTypes.Add(Constants.AuthenticationLevels.NistTypeUri);
			fields = ((IExtensionRequest)req).Serialize(null);
			Assert.IsTrue(((IExtensionRequest)req2).Deserialize(fields, null, Constants.TypeUri));
			Assert.AreNotEqual(req, req2);
			// Now go ahead and add the doubled one so we can do our equality test.
			req2.PreferredPolicies.Add(AuthenticationPolicies.PhishingResistant);
			req2.PreferredAuthLevelTypes.Add(Constants.AuthenticationLevels.NistTypeUri);
			Assert.AreEqual(req, req2);
		}

		[Test]
		public void Serialize() {
			PolicyRequest req = new PolicyRequest();
			var fields = ((IExtensionRequest)req).Serialize(null);
			Assert.AreEqual(1, fields.Count);
			Assert.IsTrue(fields.ContainsKey("preferred_auth_policies"));
			Assert.IsEmpty(fields["preferred_auth_policies"]);

			req.MaximumAuthenticationAge = TimeSpan.FromHours(1);
			fields = ((IExtensionRequest)req).Serialize(null);
			Assert.AreEqual(2, fields.Count);
			Assert.IsTrue(fields.ContainsKey("max_auth_age"));
			Assert.AreEqual(TimeSpan.FromHours(1).TotalSeconds.ToString(CultureInfo.InvariantCulture), fields["max_auth_age"]);

			req.PreferredPolicies.Add("http://pol1/");
			fields = ((IExtensionRequest)req).Serialize(null);
			Assert.AreEqual("http://pol1/", fields["preferred_auth_policies"]);

			req.PreferredPolicies.Add("http://pol2/");
			fields = ((IExtensionRequest)req).Serialize(null);
			Assert.AreEqual("http://pol1/ http://pol2/", fields["preferred_auth_policies"]);

			req.PreferredAuthLevelTypes.Add("http://authtype1/");
			fields = ((IExtensionRequest)req).Serialize(null);
			Assert.AreEqual(4, fields.Count);
			Assert.IsTrue(fields.ContainsKey("auth_level.ns.alias1"));
			Assert.AreEqual("http://authtype1/", fields["auth_level.ns.alias1"]);
			Assert.IsTrue(fields.ContainsKey("preferred_auth_level_types"));
			Assert.AreEqual("alias1", fields["preferred_auth_level_types"]);

			req.PreferredAuthLevelTypes.Add(Constants.AuthenticationLevels.NistTypeUri);
			fields = ((IExtensionRequest)req).Serialize(null);
			Assert.AreEqual(5, fields.Count);
			Assert.IsTrue(fields.ContainsKey("auth_level.ns.alias2"));
			Assert.AreEqual("http://authtype1/", fields["auth_level.ns.alias2"]);
			Assert.IsTrue(fields.ContainsKey("auth_level.ns.nist"));
			Assert.AreEqual(Constants.AuthenticationLevels.NistTypeUri, fields["auth_level.ns.nist"]);
			Assert.AreEqual("alias2 nist", fields["preferred_auth_level_types"]);
		}
	}
}
